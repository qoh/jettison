# Jettison

## What is Jettison?

Jettison is a "basic" JSON parser that supports every documented JSON data structure (object, array, string, int, float, false, true, null) except for NaN, Infinity and -Infinity. It provides (currently) the following two functions (obviously defines more, but these are the functions to use):

**json_load( file )**  
Wrapper to read file and pass it to json_loads.

**json_loads( json )**  
Parses json and returns the data (see "parse output" section) or -1 if an error occurs while parsing.

Here's an example:

    ==>echo( json_loads( "-5.3" ) );
    -5.3
    ==>echo( json_loads( "\"hello \\\"stranger\\\", have a nice stay\"" ); );
    hello "stranger", have a nice stay
    ==>echo( json_loads( "[1, 2, 3, \"foo\"]" ) );
    9627
    ==>echo( isObject( 9627 ) SPC 9627.class );
    1 JSArray
    ==>9627.echoDump();
    JSArray(9627) - 4 items
     0: 1
     1: 2
     2: 3
     3: foo

## Parse Output

What Jettison returns is a representation of the data in the format that would normally be used by developers for storing it in TorqueScript. Here's how each different data structure is stored:

* object  
This data structure is stored as a ScriptObject with the class JSObject (see reference further down the page), which implements simple dictionary (key -> value) functionality.
* array  
This data structure is stored as a ScriptObject with the class JSArray (see reference further down the page), which implements simple list (value, value, value) functionality.
* string  
This data structure is stored literally, using TS' natural data representation, e.g. "foo".
* int/float  
This data structure is stored literally, using TS' natural data representation, e.g. "5" or "-3.7".
* false/true  
This data structure is stored literally, using TS' natural data representation, e.g. "1" or "0".
* null  
This data structure is stored literally, using TS' natural data representation, e.g. "".

In objects or arrays, values which are also objects or arrays are stored as a number, referencing the object ID. To check if it's an actual JSON object, you can use **if ( %value.json )**.

## JSObject Function Reference

**JSObject.onAdd()** (internal)  
Defines the *json* field and initializes the number of keys (*count* field) to 0.

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

**JSObject.echoDump()**  
Displays a nicely formatted list of all keys and their values in the console (for debugging).

**JSObject.killTree()**  
Deletes itself and any objects referenced in key values (useful for cleaning up after being done with the data).

## JSArray Function Reference

**JSArray.onAdd()** (internal)  
Defines the *json* field and initializes the number of values (*count* field) to 0.

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

**JSArray.echoDump()**  
Displays a nicely formatted list of all values and their index in the console (for debugging).

**JSArray.killTree()**  
Deletes itself and any objects referenced in key values (useful for cleaning up after being done with the data).

## Credits

* Jettison – Port
* JSON – Douglas Crockford