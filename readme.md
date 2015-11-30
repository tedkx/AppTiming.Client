#AppTiming Client
###A C# client for calling [apptiming](https://github.com/tedkx/apptiming) services.

##Installation
----
* Install with nuget

OR

* Download and build

##Usage
----
Just initialize an AppTimingClient object with the desired API key **apptiming** endpoint.
Then either call Start(), run operation then call End()
OR
Call TimeOperation() passing the method to run

API keys refer to apps -not users- and can be managed from **apptiming** server.