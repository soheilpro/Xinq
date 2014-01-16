# Xinq
Xinq makes it extremely easy to manage database queries inside Visual Studio.

## Screenshots
![Screenshot](Screenshot.png)

## Download

- Visual Studio 2013: [xinq-v1.0.2-vs12.vsix](https://github.com/soheilpro/Xinq/releases/download/v1.0.2-vs12/xinq-v1.0.2-vs12.vsix)
- Visual Studio 2010/2012: [xinq-v1.0.2-vs10_11.vsix](https://github.com/soheilpro/Xinq/releases/download/v1.0.2-vs10_11/xinq-v1.0.2-vs10_11.vsix)

## Install
Double-click the downloaded .vsix file to install it.

## Usage

1. Add an **XML Integrated Query** file to your project.
2. Use the presented designer to add and edit queries.
3. Reference queries in your code:
   ```
   command.CommandText = UserQueries.GetNewUsers.Text;
   ```

## Version History
+ **1.0.2**
	+ Initial release

## Author
**Soheil Rashidi**

+ http://soheilrashidi.com
+ http://twitter.com/soheilpro
+ http://github.com/soheilpro

## Copyright and License
Copyright 2014 Soheil Rashidi

Licensed under the The MIT License (the "License");
you may not use this work except in compliance with the License.
You may obtain a copy of the License in the LICENSE file, or at:

http://www.opensource.org/licenses/mit-license.php

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
