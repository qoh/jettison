# Jettison

## What is Jettison?

Jettison is a "basic" JSON parser that supports every documented JSON data structure (object, array, string, int, float, false, true, null). All data is represented as objects unless the *fast* attribute is *true*. Here's an example of using it:

    ==>echo( json_loads( "-5.3" ) );
    9625
    ==>echo( 9335.describe() );
    number[9335] = -5.3
    
    ==>echo( json_loads( "[1, 2, 3, \"foo\"]" ) );
    9627
    ==>echo( 9627.type );
    array
    
    ==>echo( 9627.describe() );
    array[9627] - 4 items
     0: number[9626] = 1
     1: number[9628] = 2
     2: number[9629] = 3
     3: string[9630] = foo
    
    ==>echo( json_dumps( 9627 ) );
    [1,2,3,"foo"]

## Parse Output

By default, Jettison will represent JSON data as a ScriptObject with a class matching **JS***. Here's what each JSON type is stored as:

* object  
Objects are stored using the class JSObject, which implements basic dictionary (key -> value) functionality.
* array  
Arrays are stored using the class JSArray, which implements basic list (value, value) functionality.
* string  
Strings are stored using the class JSString, which simply stores the value on the *value* member field.
* int/float  
Numbers are stored using the class JSNumber, which simply stores the value on the *value* member field."-3.7".
* false/true  
Boolean values are stored using the class JSBool, which simply stores the value on the *value* member field.
* null  
A null value is stored using the class JSNull.

In objects or arrays, values which are also objects or arrays are stored as a number, referencing the object ID. To check if it's an actual JSON object, you can use **if ( %value.json )**.

## Function Reference

**json_dump( data, file )**  
Wrapper to invoke *json_dumps* with *data* and save the output to *file*.

**json_dumps( data )**  
Serializes *data* into a JSON string. Exact intended value types cannot be determined, it's based on guessing. See below.

**json_load( file[, fast] )**  
Wrapper to read *file* and pass it to *json_loads*. If *fast* is specified, then literal data will be used to represent the parsed output instead of objects.

**json_loads( json[, fast] )**  
Parses *json* and returns the data (see "parse output" section) or -1 if an error occurs while parsing. If *fast* is specified, then literal data will be used to represent the parsed output instead of objects.

**Notes about *fast*:**  
By default, Jettison will create objects to represent every data structure found in the JSON data in order to prevent "namespace collisions", as in, something such as a parsed number being the same as a registered object ID. In that case, checking if it was an array could in some cases be true (6449.type $= "array"). Representing all data as an object affects the performance quite noticeably though, so this option allows you to disable representing data as objects.

**Note about *json_dumps*:**  
If you're using the *fast* attribute, it cannot determine the exact type of value you're intending to be using. Instead, it inspects the value and does a few tests to try to find out what type of value it is. Here's the exact "order of tests":

* If it's an object with the class *JSObject*, treat it as an object.
* If it's an object with the class *JSArray*, treat it as an array.
* If it's a blank string, treat it as *null*.
* If it can be represented as a number in JSON, treat it as a number.
* If all else fails, treat it as a string.

## Shared Object Methods

**x.onAdd()** (internal)  
Initializes a few object fields to standard values.

**x.describe()**  
Returns a description of the object as a string with the object ID, object type and value. For objects and arrays, it includes a list of key -> value pairs (for objects) or a list of values (for arrays). This method is for debugging purposes and is available for all objects.

**x.killTree()**  
Deletes itself and all objects it contains. This method is for result cleanup purposes and is available for all objects.

**x.set( value )**  
Updates the value of the object to *value*. This method is available for strings, numbers and boolean values.

**x.get()**  
Returns the value of the object (also available under the field *value*). This method is available for strings, numbers and boolean values.

## JSObject Method Reference

**JSObject.size()**  
Returns the total number of unique keys stored in the object.

**JSObject.clear()**  
Removes all keys (and their associated value – runs *killTree* on values if they are JSON objects).

**JSObject.hasKey( key )**  
Returns *true* or *false*, depending on whether *key* is stored on the object.

**JSObject.removeKey( key )**  
Removes *key* (and it's associated value – runs *killTree* on the value if it is an JSON object) if it is stored on the object, returns *true* or *false* depending on whether *key* was previously stored on the object.

**JSObject.get( key[, default] )**  
If *key* is stored on the object, returns the value. Otherwise, returns *default* (a blank string by default).

**JSObject.set( key, value )**  
Adds *key* to the object if it was not already stored there and updates the value of it to *value*.

## JSArray Method Reference

**JSArray.size()**  
Returns the total number of values stored in the object.

**JSArray.get( index )**  
Returns the value stored at *index* on the object (or a blank string if it's out of range).

**JSArray.clear()**  
Removes all values (runs *killTree* on values if they are JSON objects).

**JSArray.append( value )**  
Stores *value* on the object (duplicates allowed).

**JSArray.remove( value )**  
Removes *value* from the object (runs *killTree* on the value if it is an JSON object), returns *true* or *false* depending on whether *value* was previously stored on the object.

**JSArray.contains( value )**  
Returns *true* or *false*, depending on whether *value* is stored on the object.

## Credits

* Jettison – Port
* JSON – Douglas Crockford