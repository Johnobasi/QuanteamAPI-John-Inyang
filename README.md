**# QuanteamAPI-John-Inyang**
This is API to fullfil job asessment for Quanteam job with her cient, SANTANDER BANK
This project is develoepd using ASP.NETCore 6
How to run this project.
Steps:
1. Clone repo
2. Restore nuget packages and ensure your machine is running on AS.NET COre 6
3. Run the API and use swagger on browser or postman

**Approach Used.**
1. I used IHttpClient to consume the APIs
2. Kept he response ina a class and later serialized as you can see on the code
3. I wrote Unit test for XUnit framework - tested two cases - please refer to code.

**Future improvement**
Given time, i would have done the following better.
1.  Create a static class to handle depenndency injection and then inject in program class.
2.  Abstract logic into interface and implement same.
3. Use CQRS pattern to seperate operations and keep code clean and tidy.
4. Use Option pattern to read configurations from AppsSettings
