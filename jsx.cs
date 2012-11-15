// Initialization methods.

function JSObject::onAdd( %this )
{
	%this.count = 0;
	%this.json = true;
	%this.type = "object";
}

function JSArray::onAdd( %this )
{
	%this.count = 0;
	%this.json = true;
	%this.type = "array";
}

function JSString::onAdd( %this )
{
	%this.json = true;
	%this.type = "string";
}

function JSNumber::onAdd( %this )
{
	%this.json = true;
	%this.type = "number";
}

function JSBool::onAdd( %this )
{
	%this.json = true;
	%this.type = "bool";
}

function JSNull::onAdd( %this )
{
	%this.json = true;
	%this.type = "null";
}

// killTree methods.

function JSObject::killTree( %this )
{
	for ( %i = 0 ; %i < %this.count ; %i++ )
	{
		if ( %this.value[ %this.key[ %i ] ].json )
		{
			%this.value[ %this.key[ %i ] ].killTree();
		}
	}

	%this.schedule( 0, "delete" );
}

function JSArray::killTree( %this )
{
	for ( %i = 0 ; %i < %this.count ; %i++ )
	{
		if ( %this.value[ %i ].json )
		{
			%this.value[ %i ].killTree();
		}
	}

	%this.schedule( 0, "delete" );
}

function JSString::killTree( %this )
{
	%this.schedule( 0, "delete" );
}

function JSNumber::killTree( %this )
{
	%this.schedule( 0, "delete" );
}

function JSBool::killTree( %this )
{
	%this.schedule( 0, "delete" );
}

function JSNull::killTree( %this )
{
	// This is static; don't get rid of it.
}

// describe methods.

function JSObject::describe( %this, %child )
{
	%string = "object[" @ %this.getID() @ "] - " @ %this.count @ " keys";

	for ( %i = 0 ; %i < %this.count && !%child ; %i++ )
	{
		%string = %string @ "\n " @ %this.key[ %i ] @ ": ";

		if ( %this.value[ %this.key[ %i ] ].json )
		{
			%string = %string @ %this.value[ %this.key[ %i ] ].describe( true );
		}
		else
		{
			%string = %string @ %this.value[ %this.key[ %i ] ];
		}
	}

	return %string;
}

function JSArray::describe( %this, %child )
{
	%string = "array[" @ %this.getID() @ "] - " @ %this.count @ " items";

	for ( %i = 0 ; %i < %this.count && !%child ; %i++ )
	{
		%string = %string @ "\n " @ %i @ ": ";

		if ( %this.value[ %i ].json )
		{
			%string = %string @ %this.value[ %i ].describe( true );
		}
		else
		{
			%string = %string @ %this.value[ %i ];
		}
	}

	return %string;
}

function JSString::describe( %this )
{
	return "string[" @ %this.getID() @ "] = " @ %this.value;
}

function JSNumber::describe( %this )
{
	return "number[" @ %this.getID() @ "] = " @ %this.value;
}

function JSBool::describe( %this )
{
	return "bool[" @ %this.getID() @ "] = " @ ( %this.value ? "true" : "false" );
}

function JSNull::describe( %this )
{
	return "null[" @ %this.getID() @ "]";
}

// Private JSObject methods.

function JSObject::size( %this )
{
	return %this.count;
}

function JSObject::clear( %this )
{
	for ( %i = 0 ; %i < %this.count ; %i++ )
	{
		if ( %this.value[ %this.key[ %i ] ].json )
		{
			%this.value[ %this.key[ %i ] ].killTree();
		}

		%this.value[ %this.key[ %i ] ] = "";
		%this.key[ %i ] = "";
	}

	%this.count = 0;
}

function JSObject::hasKey( %this, %key )
{
	for ( %i = 0 ; %i < %this.count ; %i++ )
	{
		if ( %this.key[ %i ] $= %key )
		{
			return true;
		}
	}

	return false;
}

function JSObject::removeKey( %this, %key )
{
	%found = false;

	for ( %i = 0 ; %i < %this.count ; %i++ )
	{
		if ( %found )
		{
			%this.key[ %i ] = %this.key[ %i + 1 ];
		}
		else
		{
			%found = true;
			%i--;

			continue;
		}
	}

	if ( %found )
	{
		if ( %this.value[ %key ].json )
		{
			%this.value[ %key ].killTree();
		}

		%this.count--;
		%this.value[ %key ] = "";
		%this.key[ %this.count ] = "";
	}

	return %found;
}

function JSObject::get( %this, %key, %default )
{
	if ( %this.hasKey( %key ) )
	{
		return %this.value[ %key ];
	}
	else
	{
		return %default;
	}
}

function JSObject::set( %this, %key, %value )
{
	%this.value[ %key ] = %value;

	for ( %i = 0 ; %i < %this.count ; %i++ )
	{
		if ( %this.key[ %this.count ] $= %key )
		{
			%found = true;
			break;
		}
	}

	if ( !%found )
	{
		%this.key[ %this.count ] = %key;
		%this.count++;
	}
}

// Private JSArray methods.

function JSArray::size( %this )
{
	return %this.count;
}

function JSArray::get( %this, %index )
{
	return %this.value[ %index ];
}

function JSArray::clear( %this )
{
	for ( %i = 0 ; %i < %this.count ; %i++ )
	{
		if ( %this.value[ %i ].json )
		{
			%this.value[ %i ].killTree();
		}

		%this.value[ %i ] = "";
	}

	%this.count = 0;
}

function JSArray::append( %this, %value )
{
	%this.value[ %this.count ] = %value;
	%this.count++;
}

function JSArray::remove( %this, %value )
{
	%found = false;

	for ( %i = 0 ; %i < %this.count ; %i++ )
	{
		if ( %found )
		{
			%this.value[ %i ] = %this.value[ %i + 1 ];
		}
		else
		{
			%found = true;
			%i--;

			continue;
		}
	}

	if ( %found )
	{
		if ( %value.json )
		{
			%value.killTree();
		}

		%this.count--;
		%this.value[ %this.count ] = "";
	}

	return %found;
}

function JSArray::contains( %this, %value )
{
	for ( %i = 0 ; %i < %this.count ; %i++ )
	{
		if ( %this.value[ %i ] $= %value )
		{
			return true;
		}
	}

	return false;
}

// Private JSString methods.

function JSString::set( %this, %value )
{
	%this.value = %value;
}

function JSString::get( %this )
{
	return %this.value;
}

// Private JSNumber methods.

function JSNumber::set( %this, %value )
{
	%this.value = ( strLen( %value ) ? %value : 0 );
}

function JSNumber::get( %this )
{
	return %this.value;
}

// Private JSBool methods.

function JSNumber::set( %this, %value )
{
	%this.value = ( %value ? 1 : 0 );
}

function JSNumber::get( %this )
{
	return %this.value;
}