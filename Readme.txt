Welcome to the setup readme, this document will describe how to set up a server node or a client node.

[General]
All solutions are developed in monodevelop 5 and so the simplest method of setting this up would be to install monodevelop 5 and xsp4 web server. "Xsp4 web server" is a simple ASP.NET web server and is used by monodevelop to actually launch our client or server project. The server application (TrustlessAPI) must be run by setting the directory to TrustlessAPI in the terminal and write xsp4. The reason for this is that running it within monodevelop would configure xsp4 to not allow external connections hence making it impossible for clients to connect.
To setup monodevelop 5 and xsp4 in the terminal, write the following.

sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb http://download.mono-project.com/repo/debian wheezy main" | sudo tee /etc/apt/sources.list.d/mono-xamarin.list
sudo apt-get update
sudo apt-get install monodevelop


To install xsp4 do the following:
sudo apt-get install mono-xsp4

Then execute the following to avoid a general ubuntu acccess issue with mono.
sudo mkdir /etc/mono/registry
sudo chmod uog+rw /etc/mono/registry

Both the client and server will need MultiChain installed, to install this execute the following.
su (enter root password)

cd /tmp
wget http://www.multichain.com/download/multichain-1.0-alpha-21.tar.gz
tar -xvzf multichain-1.0-alpha-21.tar.gz
cd multichain-1.0-alpha-21
mv multichaind multichain-cli multichain-util /usr/local/bin


[Server]
If you are at IT university of Copenhagen you may be able to test the client without this step as a server is already running there and the client is set up to connect it by default. In that case skip this section and read [Client].

First setup a MySQL database that reflects that of the research paper. Then configure web.config of TrustlessAPI with a correct connectionstring that maches login cridentials. Then setup the MultiChain server node by creating a new chain called trustChain, execute the following.

1) execute in terminal: multichain-util create trustChain
2) browse to Home/.multichain/trustChain and edit params.dat, make sure anyone-can-connect and anyone-can-receive is set to true.
3) execute in terminal: multichaind trustChain -daemon

Then to launch the server API, navigate to the directory of TrustlessAPI in the terminal and execute xsp4. The server API is now ready. Make sure to edit web.config inside TrustLessWebClient such that ServerUrl is the xsp4 web server accessable url that was printed when running xsp4 and edit NodeIp to match the multichaind output of the ip and port that the server node can be reached.

[Client]
The client only needs to be run directly from xsp4 within TrustLessWebClient directory or by debugging the TrustLessWebClient project in monodevelop 5. The web.config is configured (by default) to match a server set up at IT University of Copenhagen. After viewing it in the browser you will see the mainscreen, you only create a statement or view the list of recommendable statements once you have logged in. Click a login button and if you don't have an existing user, simply write the username and password that you want and click login (as the user doesn't exist it will create one as long as MultiChain is not hosting trustChain already with a public key that is tied to a specific user already). Once the login is complete you will have a reputation of initial trust level and will be presented a maximum of 3 random statements to recommend (if exist and the statements have below 10 recommendations), you will also be allowed to create statements.
You will not need a login to enter the searchscreen from the menu in the top of the website. That page will allow you to make a lookup for a statement and see its recommendations, the users that recommended and their reputation as well as a description for their recommendation.

