# eCommerce
## Link to the original team repository:
https://github.com/DorCarmi/eCommerce

## System presentation video (Hebrew)

[![Video](https://img.youtube.com/vi/ojOsFSQDm-4/0.jpg)](https://www.youtube.com/watch?v=ojOsFSQDm-4)

## Data initialization:
The file format is JSON and contains a list of action.

### Action
An action represent any valid user action of the system.
The action json data contains two fields: Action name, and the action data.

The json action format:

```
{
  "Action": action name
  "Data": data
}
```
#### Create user action:
``` 
{
   "Action": "CreateUser"
   "Data": {
   "MemberInfo": {
     "Username": The user name,
     "Email": The user email,
     "Name": The user personal name,
     "Birthday": The user birthday,
     "Address": The user address
   },
   "Password": The user password
}
```

#### Member Actions:
Member action represent any valid member request.  
They include the username and password in order to login and run the actions  
in addition to the basic action format

``` 
{
  "Action": "MemberAction",
  "Data" : {
    "Username": The user name,
    "Password": The user password,
    "Actions":  A list action to preform
    }
  }
```

##### Valid Member actions:

##### Open store:
```
{
  "Action": "OpenStore",
  "Data": {
    "StoreName": The store name
  }
}
```
##### Add item to store:
```
{
  "Action": "AddItem",
  "Data": {
    "ItemName": The item name,
    "StoreName": The item store name,
    "Amount": The amount of the item,
    "Category": The item category,
    "KeyWords": The keyworkds for search,
    "PricePerUnit": The item price
  }
}
```

##### Appoint manager to store:
```
{
  "Action": "AppointManager",
  "Data": {
    "Manager": The user name of the manager,
    "Store": The store name,
    "Permissions": List of permissions to give to the manager
  }
}
```
    
