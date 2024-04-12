# ASP.NET Web API Source Generator

Quickly set up a mock API using a Roslyn-based source generator designed to read OpenAPI specifications. By simply cloning the repository, building & running the project with a specified YAML file, you can have a fully functional mock API ready in seconds.

## quick start

Follow these steps to get your mock API server up and running:

### step 1: clone the repository

Clone this repository to your local machine using:

```bash
git clone https://github.com/cristipufu/aspnetcore-openapi-source-generator.git
cd aspnetcore-openapi-source-generator/src/OpenApi.WebApi
```
### step 2: run the project
Run the project by specifying the path to your OpenAPI specification YAML file:

```bash
dotnet run -p:OpenApiSpecPath="path_to_openapi_spec.yaml"
```

Replace `path_to_openapi_spec.yaml` with the path to your OpenAPI YAML file. This command builds and runs the project, generating the necessary API endpoints as defined in your YAML file.

### step 3: browse the API
After the application starts, it will display the URL where the server is hosted. Append `/swagger/index.html` to this URL in your browser to view and interact with your newly created mock API through the Swagger UI.
```bash
http://localhost:5287/swagger/index.html
```

## contributing
Contributions are welcome! Feel free to fork this repository and submit pull requests, or open issues to suggest improvements or report bugs.

## license
This project is licensed under the MIT License.

## support
If you need help or have any questions about using this tool, please open an issue in the repository.
