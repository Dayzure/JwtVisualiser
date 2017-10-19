## JwtVisualiser
Analyse JWT Tokens quickly while debugging your application. This is a quick and very simple custom visualiser for Visual Studio 2017 that targets string types.

## Installation
Simply clone/fork the repository, build and copy all the assemblies under 

  ` My Documents\ VisualStudioVersion \Visualizers `

For more information check [the Visual Studio documentation](https://docs.microsoft.com/en-us/visualstudio/debugger/how-to-install-a-visualizer). 

 > Note. It will not work in a subfolder. You need to place all the assemblies in the designated **Visualisers** folder.

 ## Use
 When you install the extension, simply hover any string variable or property and click on the magnifier glass (or the little arrow to make sure the *Jwt Token Visualiser* is selected)

 ![Jwt Token Visualiser](https://i.imgur.com/dVAoExv.png)

### Token Inspection
![Inspect content of your Token](https://i.imgur.com/uTPxQEG.png)

### API Helper
Try the token aginst your API and see the result immediately while debugging
![API Test](https://i.imgur.com/iMHlMzW.png)