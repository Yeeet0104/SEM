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
   > - First load might be slower, because it will pull the data from website into vector.db (sqlite)
   > - Mostly the response is not good enough, the data need to be cleaner and structured instead of directly get the html from the website

