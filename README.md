# client-server-demo
Demo of a client / server / directory server in c#

+ Client can connect to a server with IP:Port
+ Server can accept multiple client connections
+ Clients can send message to the server
+ The server can broadcast messages back to all clients

# Running the examples

1. (Optional) start the ServerDirectory first
2. Start a single instance of the Server
3. Start multiple instance of the Client

Clients will each send a messages each 2s to the server which will broadcast it back to all clients.

### TODO
1. Better error handling, serialization (MessagePack)
2. ServerDirectory (Not fully implemented yet)
	+ Passing the serialized server details
	+ Fetching the list of serialized servers details
	+ Pinging the server each X seconds to detect expired server

3. UDP Support for GameServer and GameClient as fallback from TCP (optional)
	+ UDP hole punching
