# YottoBus
NetMQ based, distributed, decentralized intranet service bus with service discovery 

## Introduction
The perpose of this project is to make it extremely simple to build relatively small distributed systems in the intranet space. 
Suppose you have several services on one machine, that would like to comminucate with each other. You have quite a few options for that (pipes, sockets and so on). On the localhost, you may know all the services and their endpoints (just ports in this case), so this is not a big deal.

Now, suppose that you have several machines with their own services, and all of this services would like to communicate with each other. The only option is using your LAN to connect this services with each other, and also there a quite a few good options, using Zookeper or RabbitMq or some other cool modern communication tools/framework.

So, what is YottoBus for? It is a micro framework, seating between ZeroMQ library and Akka.NET actors framework, that makes it extremely easy to allow services in one local network to communicate with each other. It automates lots of work on configuration and other setup you would need to perform with any other solutions - and in the same time, it is flexible enough to adjust it for your architecture.

## How it works?
This is ZeroMQ based service bus, inspired by chapter 8 of brilliant ZeroMQ Guide. It uses Publish-Subscribe communication pattern to build peer-to-peer network, where each peer can either publish message to all connected peers, or send message to one of the peer. Each peer has information about connected peers list, and can receive events about other peer connecting/disconnecting.

### Layered communication structure
Actually, there are three kinds of entites here:
* Message Handlers
* Peers
* Proxies

On each machine, you will need the Proxy service running (but you may host it in your own process). Proxies are responsible for exchanging messages between nodes and auto discovering other nodes - so proxies are forming a cluster. They are also responsible for connection recovery in case of network failure. 

Peers should be instanciated in your own processes, and they are sending/receiving messages to/from each other. You can think about them as of client for service bus usage. In particular, peer is sending message to his local proxy service, the proxy retransmits message to some other proxies (not just to all of them - proxies are smart), and each of that proxies deliver message to it's peers (to thows who are subscribed for that type of messages).

And at the end, here are message handlers are comming. This are just your classes that implement IMessageHandler<MessageType> interface. Each instance of handler class shoult be registered in one of the peers to receive messages of supported type from them.

So, basicly, there are proxies, which communicating with each other, and a bunch of peers connected to their proxy, and a bunch of message handlers registered for each of this peers. That is what we call layered structure.

## Quick Start

So, to create something working, we will need to perform 5 steps

### Step 1: Set up new project
1. Create an empty console project
2. Install NuGet package YottoBus from MyGet:
* Add my feed: https://www.myget.org/F/igorfedchenko/api/v3/index.json (NuGet V3) or https://www.myget.org/F/igorfedchenko/api/v2 (NuGet V2)
* Install-Package Yotto.ServiceBus
* Install-Package Yotto.ServiceBus.Proxy

### Step 2: Define some message type

The message should be POCO object with public default constructor, for example:

```cs
public class MyMessage
{
  public string Content { get; set; }
}
```

Message should be seriallizable with JSON.NET serializer.

### Step 3: Define your message handler class

Message handler class is just any class implementing IMessageHandler<TMessage> interface. Also, it should regiser himself to peer instance to receive messages from it.

```cs
class MyMessageHandler : IMessageHandler<MyMessage>
{
  public MyMessageHandler(IPeer peer)
  {
    peer.Register(this);
  }
  
  public void Handle(TMessage @event, PeerIdentity sender)
  {
    // Your handling code here.
    // @event is published message
    // sender is a peer who has sent this message - stores all metadata of that peer
  }
}
```

### Step 4: Create you peer instance, and connect it to the bus

First of all, you will need to describe the bus for your peers - that is, what settings will be applyed to any instanciated peer. We use Builder pattern for this, checkout YottoBusFactory methods for details.
Then, you will need to create your first peer. While there are multiple overloads for CreatePeer method, we recommend at least to specify peer name (may not be unique, but supposed to be human readable) and peer context. 
The context is like a namespase, of like a private subnet for peers - that is, only peers in the same context can discover and communicate with each other. By default, context will be set to empty string, which is just yet another separate context.

```cs
var bus = new YottoBusFactory().Create();

using (var peer1 = bus.CreatePeer("MyPeer", "SomeContext"))
{
  peer1.Connect();
  
  // Wait here untic program terminates
}
```

That created instance should be passed to your message handler. One of the good approaches would be to use IoC container for that.

In general, you do not need to instanciate more then one peer& You may need to have several peers when you would like to work in several contexts at once.

It is possible to save custom metadata for the peer - see additional parameters in CreatePeer method. This metadata will be available for message handlers from PeerIdentity object.

### Step 5: Start the proxy

After installation of the Yotto.ServiceBus.Proxy package, you will have Yotto.ServiceBus.Proxy.exe in the package folder under packages solution directory. It uses TopShelf and TopShelf.Unix to make it possible to install it as a service on Windows or Linux:

For Windows (from cmd with admin permissions):
* Yotto.ServiceBus.Proxy.exe install
* Yotto.ServiceBus.Proxy start

For Linux:
* sudo mono Yotto.ServiceBus.Proxy.exe install
* sudo mono Yotto.ServiceBus.Proxy start

Or you could just start it as a regular console app: call Yotto.ServiceBus.Proxy.exe for Windows or mono Yotto.ServiceBus.Proxy.exe under Linux.

### That's it!
Now, your peer can publish messages and your handler will receive them!
