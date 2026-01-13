# How to run the application

This short file will explain how to run this application.

First of all, you will need to clone the GitHub Repository with the command:
```
git clone 
```

Next, you need to move to the /src/Festivo-Application/ folder because this is where the docker-compose.yaml is located:
```
cd ./src/Festivo-Application
```

From there, you will just have to start the docker-compose with:
```
docker-compose up -d --build
```

The service should start and notify you, when done. If you want to stop it again, you may use:
```
docker-compose down
```
