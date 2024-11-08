# Software Evolution and Maintenance (Task 2)

## List of function

1. Chatbot
2. Event Calendar
3. Integrated Scholarship and Financial Aid Calculator
4. Feedback Form Tool
5. Online Integrated Development Environment
6. Meeting Schedule Calendar
7. Visit Request
8. Virtual Tour

##  Instruction

### Chatbot

1. Download [ollama](https://ollama.com/download)

2. Download `ollama3.1` (llm) & `nomic-embed-text` (embedding)

   - > **Note :** 
     >
     > - You may want to change the path to save the model, simply open Environment Variable, add new user variables `OLLAMA_MODELS`, and put the path in Value
     > - Also make sure the ollama server is running, else ollama cmd will not work

   - Open terminal run `ollama pull llama3.1` & `ollama pull nomic-embed-text` 

   - Then gaodim aldy

3. Now you can run the program, try out the chatbot (should contain a button to navigate to the page)

   > **NOTE :** 
   >
   > - Checkout the **terminal** for the status, if you are not interested in the code.
   > - First time run might be slower, because it will load model from disk
   > - Website data will pull into the vector.db if the collection is not exist.
   > - Mostly the response is not good enough, the data need to be cleaner and structured instead of directly get the html from the website

4. **Stop the `Ollama` server manually**, so it will not retain in memory!!



### Instruction for database ( SQL LITE) 
1. go console type these
   - `dotnet tool install --global dotnet-ef`
   - `dotnet ef database update`


### Instruction for Online IDE
1. Install this NuGet Packages for docker in Package Manager Console
   > - Install-Package Docker.DotNet

2. Install the [Docker Desktop](https://docs.docker.com/desktop/install/windows-install/)

3. Go on docker desktop > settinges (gear icon on top right of the app) > Builder. Make it it shows desktop linux.
4. Run code in Package Manager Console
   - `docker pull python:3.9-slim`
