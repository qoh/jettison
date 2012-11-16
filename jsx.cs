// onAdd method (shared).

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

// killTree method (shared).

function JSObject::killTree( %this )
{
	for ( %i = 0 ; %i < %this.count ; %i++ )
	{
		%value = %this.value[ %this.key[ %i ] ];
		%type = js_type( %value );

		if ( %type $= "object" || %type $= "array" )
		{
			%value.killTree();
		}
	}

	%this.schedule( 0, "delete" );
}

function JSArray::killTree( %this )
{
	for ( %i = 0 ; %i < %this.count ; %i++ )
	{
		%value = %this.value[ %i ];
		%type = js_type( %value );

		if ( %type $= "object" || %type $= "array" )
		{
			%value.killTree();
		}
	}

	%this.schedule( 0, "delete" );
}

// describe method (shared).

function JSObject::describe( %this, %child )
{
	%string = "object[" @ %this.getID() @ "] - " @ %this.count @ " keys";

	for ( %i = 0 ; %i < %this.count && !%child ; %i++ )
	{
		%value = %this.value[ %this.key[ %i ] ];
		%type = js_type( %value );

		%string = %string @ "\n " @ %this.key[ %i ];
		%string = %string @ " (" @ %type @ "): ";
		%string = %string @ %value;
	}

	return %string;
}

function JSArray::describe( %this, %child )
{
	%string = "array[" @ %this.getID() @ "] - " @ %this.count @ " items";

	for ( %i = 0 ; %i < %this.count && !%child ; %i++ )
	{
		%value = %this.value[ %i ];
		%type = js_type( %value );

		%string = %string @ "\n " @ %i;
		%string = %string @ " (" @ %type @ "): ";
		%string = %string @ %value;
	}

	return %string;
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
		%value = %this.value[ %this.key[ %i ] ];
		%type = js_type( %value );
		
		if ( %type $= "object" || %type $= "array" )
		{
			%value.killTree();
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
		%value = %this.value[ %key ];
		%type = js_type( %value );
		
		if ( %type $= "object" || %type $= "array" )
		{
			%value.killTree();
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
	%old = %this.value[ %key ];
	%type = js_type( %old );
		
	if ( %type $= "object" || %type $= "array" )
	{
		%old.killTree();
	}

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
		%value = %this.value[ %i ];
		%type = js_type( %value );

		if ( %type $= "object" || %type $= "array" )
		{
			%value.killTree();
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
		%type = js_type( %value );

		if ( %type $= "object" || %type $= "array" )
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