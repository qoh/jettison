# Jettison

## What is Jettison?

Jettison is a "basic" JSON parser that supports every documented JSON data structure (object, array, string, int, float, false, true, null). Here's an example of using it:

    ==> $a = json_loads( "-5.3" );

    ==> echo( $a );
    -5.3

    ==> echo( js_type( $a ) );
    number
    
    ==> $a = json_loads( "[57, 33, 90, \"foo\"]" );

    ==> echo( $a );
    9627

    ==> echo( js_type( $a ) );
    array
    
    ==>echo( $a.describe() );
    array[9627] - 4 items
     0: 57
     1: 33
     2: 90
     3: foo
    
    ==>echo( json_dumps( $a ) );
    [57,33,90,"foo"]

## Parse Output

Jettison represents JSON data as raw strings where possible. Objects and arrays, though, are stored as ScriptObjects with a specific class. Below is a complete listing of how every data structure would be represented.

* object  
An object is stored as a "JSObject", which is a class that implements basic dictionary (key -> value) functionality.
* array  
An array is stored as a "JSArray", which is a class that implements basic list (value, value) functionality.
* string  
A string is stored as a raw string, such as "foo".
* int/float  
A number is stored as a raw string, such as "-3.741".
* false/true  
A boolean values is stored as "0" or "1".
* null  
A null value is stored as "".

## Function Reference

**json_dump( data, file )**  
Wrapper to invoke *json_dumps* with *data* and save the output to *file*.

**json_dumps( data )**  
Serializes *data* into a JSON string. Exact intended value types cannot be determined, it's based on guessing. See below.

**json_load( file )**  
Wrapper that reads *file* and passes it to *json_loads*.

**json_loads( json )**  
Parses *json* and returns the data (see "parse output" section) or -1 if an error occurs while parsing.

**js_object()**  
Creates a new JSON object and returns it. Used for creating new data.

**js_array()**  
Creates a new JSON array and returns it. Used for creating new data.

**Note about *json_dumps*:**  
It cannot determine the exact type of value you're intending to be using. Instead, it inspects the value and does a few tests to try to find out what type of value it is. Here's the exact "order of tests":

* If it's an object, treat it as the object type.
* If it's a blank string, treat it as *null*.
* If it can be represented as a number in JSON, treat it as a number.
* If all else fails, treat it as a string.

## Shared Object Methods

**x.onAdd()** (internal)  
Initializes a few object fields to standard values.

**x.describe()**  
Returns a description of the object as a string with the object ID, object type and value.

**x.killTree()**  
Deletes itself and all objects it contains.

## JSObject Method Reference

**JSObject.size()**  
Returns the total number of unique keys stored in the object.

**JSObject.clear()**  
Removes all keys (and their associated value – runs *killTree* on values that are parsed objects).

**JSObject.hasKey( key )**  
Returns *true* or *false*, depending on whether *key* is stored on the object.

**JSObject.removeKey( key )**  
Removes *key* from the object (and it's associated value – runs *killTree* on the value if it is a parsed object).

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
Removes *value* from the object (runs *killTree* on the value if it is a parsed object).

**JSArray.contains( value )**  
Returns *true* or *false*, depending on whether *value* is stored on the object.

## Credits

* Jettison – Port
* JSON – Douglas Crockford