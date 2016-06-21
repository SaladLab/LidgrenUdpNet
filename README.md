# LidgrenUdpNet

[![NuGet Status](http://img.shields.io/nuget/v/LidgrenUdpNet.svg?style=flat)](https://www.nuget.org/packages/LidgrenUdpNet/)
[![Build status](https://ci.appveyor.com/api/projects/status/ietq5cnljm94nlku?svg=true)](https://ci.appveyor.com/project/veblush/lidgrenudpnet)
[![Coverage Status](https://coveralls.io/repos/github/SaladLab/LidgrenUdpNet/badge.svg?branch=master)](https://coveralls.io/github/SaladLab/LidgrenUdpNet?branch=master)

Modified version of [lidgren-network-gen3](https://github.com/lidgren/lidgren-network-gen3)
- Support .NET 3.5 in the nuget package.
- Support UnityPackage.
- Add NetConnection.MessageHandler to handle incoming messages quickly.
- Add Connection ID to handle change of peer address under mobile environment.
