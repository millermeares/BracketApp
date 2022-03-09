# BracketApp

The hope is that this will be an app to help a friend of mine generate optimized March Madness brackets. 

SSL unsafe warning on dev fixed by:
-installing mkcert with chocolatey on system. 
-creating a certification directory in bracket-client
inside that directory:
mkcert -install
mkcert localhost


then, and here is the key part: 
in bracket-client/package.json, in the start script, set the ssl file path, ssl file key, and the https environment variable to true. 

after this, is should work. 

ALSO! The server is running on a different port because the server and client are technically separate entities. This is fixed by add a "proxy" value in bracket-client/package.json. 
Just do: 
"proxy": {server-path}



KNOWN ISSUES: 
-404s on refresh in production
