# Jettison

> A full-featured JSON parsing and serialization library for TorqueScript.

## Usage

Basic usage:

```csharp
if (jettisonParse(...)) {
  // Error position is available through $JSON::Index
  error("Failed to parse JSON: " @ $JSON::Error);
  return;
}

echo("Type: " @ $JSON::Type @ " | Value: " @ $JSON::Value);

// Cleanup is simple
if ($JSON::Type $= "object") {
  $JSON::Value.delete();
}
```

Serialization:

```csharp
%data = JettisonObject();
%data.set("position", "object", JettisonArray());
%data.position.push("number", 1);
%data.position.push("number", 2);
%data.set("health", "number", 0);
%json = jettisonStringify("object", %data);
```

```csharp
%json -> "{\"position\":[1,2],\"health\":0}"
```

Data structures:

```csharp
// Where `data` is `$JSON::Value` from parsing the below structure:
// {
//   "admins": [
//     {"name": "Bob", "level": 3},
//     {"name": "Joe", "level": 2}
//   ]
// }

data.class      -> "JettisonObject"
data.keyCount   -> 1
data.keyName[0] -> "admins"

data.value["admins"] -> [{...}, {...}]
data.admins          -> [{...}, {...}]

data.admins.length   -> 2
data.admins.value[0] -> {"name": "Bob", ...}
data.admins.value[1] -> {"name": "Joe", ...}

data.admins.value[0].name  -> "Bob"
data.admins.value[0].level -> 3
```

## Type names

All the valid type names you can expect to get from `$JSON::Type`, and which can be used as valid arguments to the various functions expecting type names:

* `"null"`
* `"boolean"`
* `"number"`
* `"string"`
* `"object"` - Value should be an object that implements `::toJSON()`

**Note**: Objects and arrays share the same type name. They can be disambiguated between using the `class` field (`"JettisonObject"` versus `"JettisonArray"`).

## API

#### `jettisonParse(text: string) -> boolean`

Attempt to parse an arbitrary JSON string, ignoring any whitespace around it.

Returns `true` if an error occurred while parsing. In this case, `$JSON::Error` will contain a short error message, and `$JSON::Index` will point to the character position where the error occurred.

If there was no error, `false` is returned, `$JSON::Value` is set to the result, and `$JSON::Type` is set to the type of value that was parsed.

Example:

```csharp
if (jettisonParse(%text)) {
  error("Parse error at " @ $JSON::Index @ ": " @ $JSON::Error);
  return;
}

echo("Parsed a " @ $JSON::Type @ ": " @ $JSON::Value);
```

The only cleanup required is when this function returns true and `$JSON::Value` is `"object"`. In this case, deleting `$JSON::Value` when it's no longer needed is all that's necessary.

---

#### `jettisonStringify(type: string, value: *) -> string`

Serialize an arbitrary value into a JSON string.

Will never fail or display errors except for the following cases:

* `type` is not a valid type name: returns a special string describing the error which is *intentionally invalid JSON*. Any attempt to parse it will fail. You can detect this by checking if the result starts with `"<"`.
* `type` is `"object"` and `value` does not implement `::toJSON()`: The typical method call failure message is displayed in the console and an empty string is returned.

Example:

```csharp
jettisonStringify("boolean", true)          -> "true"
jettisonStringify("number", 3.14)           -> "3.14"
jettisonStringify("string", "hello\nworld") -> "\"hello\\nworld\""
jettisonStringify("object", JettisonObject())   -> "{}"
```

---

#### `jettisonReadFile(filename: string) -> boolean`

#### `jettisonWriteFile(filename: string, type: string, value: *) -> string`

Helper functions for parsing JSON from/serializing JSON to a file.

`jettisonReadFile` behaves exactly like `jettisonParse`.

`jettisonWriteFile` returns `""` on success or an error message on failure.

---

#### `class JettisonObject`

> Represents a plain JSON object (key-value store).  
>
> **Note**: Keys names are case insensitive! There may be a case sensitive version (hashing keys with `sha1`) in the future, however.
>
> #### `keyCount: number`
>
> The total number of keys stored in this object.
>
> #### `keyExists[name: string]: boolean`
>
> Has a value been assigned to this key name?
>
> #### `keyName[index: number]: string`
>
> The name of the `index`th key in the list of existing keys.
> `index` must be `>= 0` and `< keyCount`.
>
> #### `type[key: string]: string`
>
> The type name of the value associated with a given key.
>
> #### `value[key: string]: *`
>
> The value associated with a given key.
>
> #### `[key: string]: *`
>
> Direct access to the value of a key by name. `.value["foo"]` can be substituted with `.foo`.
>
> **Note**: Certain keys cannot be accessed this way. Any key whose name is `"class"`, `"className"`, `"keyCount`", or whose name *starts with* any of the above field prefixes will not have a shorthand field.
>
> #### `toJSON() -> string`
>
> Serialize this object into a JSON string. Should not be used directly (use `jettisonStringify` instead).
>
> #### `set(key: string, type: string, value: *)`
>
> Add or update a key-value pair in the object.
>
> #### `remove(key: string) -> boolean`
>
> Remove a key-value pair from the object by the key name.

---

#### `class JettisonArray`

> Represents a JSON array (list/vector).
>
> #### `length: number`
>
> The total number of values in the array.
>
> #### `type[index: number]: string`
>
> The type name of the indexed value. See `value[]`.
>
> #### `value[index: number]: *`
>
> The value at a certain index (position) in the array.
> `index` must be `>= 0` and `< length`.
>
> #### `toJSON() -> string`
>
> Serialize this array into a JSON string. Should not be used directly (use `jettisonStringify` instead).
>
> #### `push(type: string, value: *)`
>
> Append a value to the end of the array, increasing `length` by one.
