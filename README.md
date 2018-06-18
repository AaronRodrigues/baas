Energy.ProviderAdapters
==========

Adapter for EHL calls 

# Prerequisites

* Windows 7.x or higher
* Git
* .NET 4.5.2 [runtime](http://www.microsoft.com/en-us/download/details.aspx?id=42642) and [SDK](http://www.microsoft.com/en-gb/download/details.aspx?id=42637)
* Visual Studio 2015 or higher
* JetBrains Resharper 2016

# Getting Started

After cloning, `cd` into the root of the repo and run `fake.bat`  
This will compile the solution and run the tests.

# Test Coverage

To generate an HTML test report:
```
fake.bat mode=Debug target=CreateTestCoverageReport
```  

The report will be output to the `coverate-report` directory.
