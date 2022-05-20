![build & test](https://github.com/vmandic/url_shortner/actions/workflows/dotnet.yml/badge.svg)

Hosted temporarily on [Azure Container Instance (Swagger UI) here](http://20.73.140.47/swagger/index.html).

# UrlShortner - a fun code challenge

This repo demonstrates my (hi I'm Vedran!) attempt at solving a coding challenge of building a simple URL shortner web service.

## OK, what is this really?

This is a coding challenge where the task is to build a simple HTTP REST API service which can do URL shortening. URL shortening is a process where a user provides a long HTTP URL (link to a web site) which is then replaced ie. shortend by a shorter variant. One could describe this service as smarter `Dictionary<string, string>` as a service where the key is the long original URL and shorter URL the pairing value or vice versa.

This repo (and its source) ie. project should present my simple and basic solution to the proposed problem.

## Some assumptions and constraints - _back of the envelope_

When designing any software solution the author(s) should take system constraints in consideration. Constraints such as disk memory size, RPS (requests per second), rate limiting, security, scalability etc. This section lays out some of them so the system does not get overengineered but can evolve if required later.

- URL links can hold one or more of the 128 chars of the ASCII charset and can be treated as varchar data of `1 byte` per char in a DB system
- number of links per day: `100.000`
- links generated per sec: `100.000 / 24 / (60*60) = 1,2 RPS`
- lets assume read is 10x ratio of write: `12 RPS`
- assuming the service will run `5 years` then the total number of links generated is `100.000 * 365 * 5 = 182,5 mil`
- average [URL is 66 characters](https://backlinko.com/search-engine-ranking) and assuming a char takes a byte the total storage in `5 years` would be `66 bytes * 182,5 mil links = 1.1 TB` of storage at least just for long URL datapoint, also we need to track ID and shortURL at most so lets make it `100 bytes` which now totals with more actual `1.7 TB` requirement

## Approach

- I'm using CRC32 that produces 8 chars fixed len hash for short URLs

## How to build, run and test?

### Pre-reqs

You can build the source on your machine if you have the matching `global.json`.NET SDK installed. You can find the required one on the [official download Microsoft site](https://dotnet.microsoft.com/en-us/download). If you have not tried .NET and C# by now, OK, well you really should. Why? Because .NET is awesome, fast, easy to learn, maintainable, ever-evolving, with a lovey bunch of folks behind it, such a great community... and yes it is cross-plat, and also blazing fast for your k8s cluster and any linux server.

### Build, run & test (Linux)

#### Build

To build the source run from root dir `$ dotnet build` - triggers Visual Studio Solution file build which builds all of the related projects (also running a NuGet package manager restore of referenced libs).

#### Run

To run the application from root dir `$ dotnet run /src/UrlShortner.HttpApi` - start the localhost server and off you are creating and sharing shorter URLs.

To run the app via Docker you can build the image yourself or just pull it from a repo, here is how to build one from project root:

```bash
> docker build -f ./src/UrlShortner.HttpApi/Dockerfile --force-rm -t urlshortnerhttpapi:latest .
```

To run the image as container on your machine on port 4000:

```bash
> docker run -d -p 4000:80 --name app1 urlshortnerhttpapi:latest
```

To pull the image use the command:

```bash
> docker pull vmandic/url_shortner:latest
```


#### Test

To run tests from root dir `$ dotnet test` - finds the test projects and executes them, more precisely should start a temp localhost server (`TestServer` does "the magic") aginst which some integration sanity tests are ran.

## Features

The applications offers two simple features:

1. Create a shortened URL via POST REST API endpoint which accepts and serves a JSON payload.

    Sample scenario

    ```bash
    > curl localhost:4000 -X POST -d '{ "url": "http://www.somelongurl.com?abc=xyz" } -H 'Content-Type: application/json'

    > { "short_url": "/abc123", "url": "http://www.somelongurl.com?abc=xyz" }
    ```

2. Load the shortened URL to get a HTTP 301 redirection response to the original long URL with a JSON payload of it included.

    Sample scenario

    ```bash
    curl -v localhost:4000/abc123
    ...
    < HTTP/1.1 301 Moved Permanently
    ...
    < Location: http://www.somelongurl.com?abc=xyz
    ...
    { "url": "http://www.somelongurl.com?abc=xyz" }
    ```

## The "smart stuff" - the problem

This system is interesting to observe from an algorithmic perspective ie. how would one implement an efficient shortening logic. The problem lies in choosing a right or optimal solution to creating a shortened URL via a hashing function. Eg. a developer could choose to produce a hash from the long URL or to generate a hash from the incrementing primary key ID numeric value. Both approaches have their own pros and cons. For example the former suffers from collision and the latter can have its short URL guessed.

At the initial moment of writing this I am not sure which one should I go for, I guess the faster to implement one, eg. a CRC32 hashing function with a check for collision in the backing store. This one would suffer on the longer run from the size of the string (ie. it matters on the long run how long the string value is).

## Author and credits

Vedran Mandić.
