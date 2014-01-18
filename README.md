SafeChat for Windows 8
========

Safely Chat on Windows 8 with Off The Record Protocol using Diffie Helman Key Exchange. OTR brings us confidentiality that the messages are encrypted. Authentication that verifies who the initiator and receiver are. Perfect Forward Secrecy so that each instant message sent is encrypted using a different encryption key. And deniability, so that the MAC keys that already have been used will not be used again.

Challenges
----------

* Creating a Windows 8 compatible OTR Library (we found a .NET Wrapper, requires rewriting the Cryptography layer to be compatible with modern UIs)
* Creating a Windows 8 XMPP Library (we found an open source version)
* Putting it all together so pure XMPP messages are encrypted using OTR
