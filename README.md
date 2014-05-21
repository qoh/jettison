# Jettison

## What is Jettison?

Jettison is a "basic" JSON parser that supports every documented JSON data structure (object, array, string, int, float, false, true, null). Here's an example of using it:

    ==> $a = parseJSON( "-5.3" );

    ==> echo( $a );
    -5.3

    ==> echo( getJSONType( $a ) );
    number
    
    ==> $a = parseJSON( "[57, 33, 90, \"foo\"]" );

    ==> echo( $a );
    9627

    ==> echo( getJSONType( $a ) );
    array
    
    ==>echo( $a.describe() );
    array[9627] - 4 items
     0: 57
     1: 33
     2: 90
     3: foo
    
    ==>echo( dumpJSON( $a ) );
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

## API Reference

See [here](https://rawgit.com/portify/jettison/master/docs/jettison.cs.html).

## Credits

* Jettison – Port
* JSON – Douglas Crockford