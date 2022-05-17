# UrlShortner - a fun code challenge

This repo demonstrates my (hi I'm Vedran!) attempt at solving a coding challenge of building an simple URL shortner web service.

## OK, what is this really?

This is a coding challenge where the task is to build a simple HTTP REST API service which can do URL shortening. URL shortening is a process where a user provides a long HTTP URL (link to a web site) which is then replaced ie. shortend by a shorter variant. One could describe this service as smarter `Dictionary<string, string>` as a service where the key is the long original URL and shorter URL the pairing value.

This repo (and its source) ie. project should present my simple and basic solution to the proposed problem.

## Some assumptions and constraints - _back of the envelope_

When designing any software solution the author(s) should take system constraints in consideration. Constraints such as disk memory size, RPS (requests per second), rate limiting, security, scalability etc. This section lays out some of them so the system does not get overengineered but can evolve if required later.

## How to build, run and test?

### Pre-reqs

You can build the source on your machine if you have the matching `global.json`.NET SDK installed. You can find the required one on the [official download Microsoft site](https://dotnet.microsoft.com/en-us/download). If you have not tried .NET and C# by now, OK, well you really should. Why? Because .NET is awesome, fast, easy to learn, maintainable, ever-evolving, with a lovey bunch of folks behind it, such a great community... and yes it is cross-plat, and also blazing fast for your k8s cluster and any linux server.

### Build, run & test (Linux shell)

#### Build

To build the source run from root dir `$ dotnet build` - triggers Visual Studio Solution file build which builds all of the related projects (also running a NuGet package manager restore of referenced libs).

#### Run

To run the application from root dir `$ dotnet run /src/UrlShortner.HttpApi` - start the localhost server and off you are creating and sharing shorter URLs.

To run the app via Docker you can build the image yourself or just pull it from the hub.

```bash
> TODO DOCKER how to...
```

#### Test

To run tests from root dir `$ dotnet test` - finds the test projects and executes them, more precisely should start a temp localhost server (`TestServer` does "the magic") aginst which some integration sanity tests are ran.

## Features

The applications offers two simple features:

1. Create a shortened URL via POST REST API endpoint which accepts and serves a JSON payload.
2. Load the shortened URL to get a HTTP 301 redirection response to the original long URL with a JSON payload of it included.

## The "smart stuff" - the problem

This system is interesting to observe from an algorithmic perspective ie. how would one implement an efficient shortening logic. The problem lies in choosing a right or optimal solution to creating a shortened URL via a hashing function. Eg. a developer could choose to produce a hash from the long URL or to generate a hash from the incrementing primary key ID numeric value. Both approaches have their own pros and cons. For example the former suffers from collision and the latter can have its short URL guessed.

At the initial moment of writing this I am not sure which one should I go for, I guess the faster to implement one, eg. a CRC32 hashing function with a check for collision in the backing store. This one would suffer on the longer run from the size of the string (ie. it matters on the long run how long the string value is).

## TODO list of stuff I need to cover here

- [ ] make sure this file is done
- [ ] write actual code, tests, dockerize

## Author and credits

Vedran Mandić (trying to be smart).
