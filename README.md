# Software Evolution and Maintenance (Task 2)

## List of function

1. Chatbot
2. ...

##  Instruction

### Chatbot

1. Download [ollama](https://ollama.com/download)

2. Download `ollama3.1` (llm) & `all-minilm` (embedding)

   - > **Note** : 
     >
     > - You may want to change the path to save the model, simply open Environment Variable, add new user variables `OLLAMA_MODELS`, and put the path in Value
     > - Also make sure the ollama server is running, else ollama cmd will not work

   - Open terminal run `ollama pull llama3.1` & `ollama pull all-minilm` 

   - Then gaodim aldy

3. Now you can run the program, try out the chatbot (should contain a button to navigate to the page)

   > **NOTE : ** 
   >
   > - Checkout the **terminal** for the status, if you are not interested in the code.
   > - First time run might be slower, because it will load model from disk
   > - Website data will pull into the vector.db if the collection is not exist.
   > - Mostly the response is not good enough, the data need to be cleaner and structured instead of directly get the html from the website

4. **Stop the `Ollama` server manually**, so it will not retain in memory!!



## Instruction for database ( SQL LITE) 
1. go console type these

dotnet tool install --global dotnet-ef

dotnet ef migrations add InitialCreate

dotnet ef database update

dotnet ef migrations add AddIdentityToAppDbContext

dotnet ef database update


## Instruction for Online IDE
1. Install this NuGet Packages for docker
   > - Install-Package Docker.DotNet
2. Install the [Docker Desktop](https://docs.docker.com/desktop/install/windows-install/)
3. Go on the tray, find whale icon right click and make sure to switch it to linux container. If you are already seeing switch to linux container, then its fine.
