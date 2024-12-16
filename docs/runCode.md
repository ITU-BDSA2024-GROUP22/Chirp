## How to make _Chirp!_ work locally
To run the system, follow these steps.

**Step 0: Ensure you have the correct softwares downloaded**

Software needed:

Rider or Visual Studio Code

.NET 8.0

If you don’t have .NET 8.0 installed, run the following command.
```
brew install dotnet@8
```


### **Step 1: Cloning the project**

Navigate to the Github repository page for Group 8:

https://github.com/ITU-BDSA2024-GROUP22/Chirp

Press on the green button '<> Code' and copy the HTTPS address

Navigate to the folder where you want to clone the repository:

```
cd path/to/your/folder
```
Use the copied repository URL:

```
git clone <repository-url>
```
### **Step 2: Open project**

Open in Rider or Visual Studio Code

Navigate to Chirp.web

```
cd src/Chirp.Web
```


### **Step 3: set user secrets**

Run these two commands to set the user secrets

```
dotnet user-secrets set "authentication_github_clientId" "Ov23liZYXvXPxOxqjMap" dotnet user-secrets set "authentication_github_clientSecret" "ab07248b11a19096e2c822b96605679072c02f74"
```

### **Step 4: run the program on localhost**

```
dotnet run
```

Click on the URL and you will be directed to the web application.

![fig##](./images/URL.png)

Be aware that when logging in, it needs your username and not e-mail.

![fig##](./images/Chirp-logIn_page.png)

To shut down the application, press Ctrl+C in the local terminal.

