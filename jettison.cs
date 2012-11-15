exec( "./jsx.cs" );

function json_match_number( %string, %index )
{
	%output = "";
	%length = strLen( %string );

	%allow = "0123456789-.";
	%minus = false;
	%stage = 0;

	for ( %index ; %index < %length ; %index++ )
	{
		%char = getSubStr( %string, %index, 1 );

		if ( strPos( %allow, %char ) < 0 )
		{
			if ( %stage == 0 )
			{
				return "";
			}

			if ( %stage == 2 )
			{
				return -1;
			}

			return %output;
		}

		if ( %char $= "-" )
		{
			if ( %minus || %stage != 0 )
			{
				return -1;
			}

			%minus = true;
		}

		if ( %char $= "." )
		{
			if ( %stage == 1 )
			{
				%stage = 2;
			}
			else
			{
				return -1;
			}
		}
		else if ( %stage == 2 )
		{
			%stage = 3;
		}

		%output = %output @ %char;

		if ( %stage == 0 )
		{
			%stage = 1;
		}
	}

	if ( strLen( %output ) && %stage != 2 )
	{
		return %output;
	}

	return -1;
}

function json_parse_string( %string, %begin, %fast )
{
	%index = %begin;
	%length = strLen( %string );

	while ( %index < %length )
	{
		%result = strPos( getSubStr( %string, %index, %length ), "\"" );

		if ( %pos < 0 )
		{
			return -1;
		}

		%before = getSubStr( %string, %index + %result - 1, 1 );

		if ( %before $= "\\" )
		{
			%index = %index + %result + 1;
		}
		else 
		{
			%string = collapseEscape( getSubStr( %string, %begin, %index + %result - %begin ) );

			if ( %fast )
			{
				return %string TAB %index + %result + 1;
			}
			else
			{
				%obj = new scriptObject()
				{
					class = "JSString";
					value = %string;
				};

				return %obj TAB %index + %result + 1;
			}
		}
	}

	return -1;
}

function json_parse_object( %string, %index, %fast )
{
	%length = strLen( %string );

	if ( %length - %index < 1 )
	{
		return -1;
	}

	%next = getSubStr( %string, %index, 1 );

	if ( %next !$= "\"" )
	{
		if ( strPos( " \t\n\r", %next ) >= 0 )
		{
			for ( %index ; %index < %length ; %index++ )
			{
				if ( strPos( " \t\n\r", getSubStr( %string, %index, 1 ) ) < 0 )
				{
					break;
				}
			}

			if ( %index >= %length )
			{
				return -1;
			}

			%next = getSubStr( %string, %index, 1 );
		}

		if ( %next $= "}" )
		{
			%object = new scriptObject()
			{
				class = "JSObject";
			};

			return %object TAB %index + 1;
		}

		return -1;
	}

	while ( true )
	{
		%value = json_parse_string( %string, %index + 1, true );

		if ( %value $= -1 )
		{
			return -1;
		}

		%key = getField( %value, 0 );
		%index = getField( %value, 1 );

		%next = getSubStr( %string, %index, 1 );

		if ( %next !$= ":" )
		{
			if ( strPos( " \t\n\r", %next ) >= 0 )
			{
				for ( %index ; %index < %length ; %index++ )
				{
					if ( strPos( " \t\n\r", getSubStr( %string, %index, 1 ) ) < 0 )
					{
						break;
					}
				}

				if ( %index >= %length )
				{
					return -1;
				}

				%next = getSubStr( %string, %index, 1 );
			}

			if ( %next !$= ":" )
			{
				return -1;
			}
		}

		%index += 1;
		%next = getSubStr( %string, %index, 1 );

		if ( strPos( " \t\n\r", %next ) >= 0 )
		{
			for ( %index ; %index < %length ; %index++ )
			{
				if ( strPos( " \t\n\r", getSubStr( %string, %index, 1 ) ) < 0 )
				{
					break;
				}
			}

			if ( %index >= %length )
			{
				return -1;
			}

			%next = getSubStr( %string, %index, 1 );
		}

		%out = json_scan_once( %string, %index, %fast );

		if ( %out $= -1 )
		{
			return -1;
		}

		%value = getField( %out, 0 );
		%index = getfield( %out, 1 );

		%next = getSubStr( %string, %index, 1 );

		if ( !isObject( %object ) )
		{
			%object = new scriptObject()
			{
				class = "JSObject";
			};
		}

		%object.set( %key, %value );

		if ( strPos( " \t\n\r", %next ) >= 0 )
		{
			for ( %index ; %index < %length ; %index++ )
			{
				if ( strPos( " \t\n\r", getSubStr( %string, %index, 1 ) ) < 0 )
				{
					break;
				}
			}

			if ( %index >= %length )
			{
				%object.delete();
				return -1;
			}

			%next = getSubStr( %string, %index, 1 );
		}

		if ( %next $= "}" )
		{
			break;
		}

		if ( %next !$= "," )
		{
			%object.delete();
			return -1;
		}

		if ( %index >= %length )
		{
			%object.delete();
			return -1;
		}

		%index++;
		%next = getSubStr( %string, %index, 1 );

		if ( strPos( " \t\n\r", %next ) >= 0 )
		{
			for ( %index ; %index < %length ; %index++ )
			{
				if ( strPos( " \t\n\r", getSubStr( %string, %index, 1 ) ) < 0 )
				{
					break;
				}
			}

			if ( %index >= %length )
			{
				%obj.delete();
				return -1;
			}
		}
	}

	return %object TAB %index + 1;
}

function json_parse_array( %string, %index, %fast )
{
	%length = strLen( %string );

	if ( %length - %index < 1 )
	{
		return -1;
	}

	%next = getSubStr( %string, %index, 1 );

	if ( strPos( " \t\n\r", %next ) >= 0 )
	{
		for ( %index ; %index < %length ; %index++ )
		{
			if ( strPos( " \t\n\r", getSubStr( %string, %index, 1 ) ) < 0 )
			{
				break;
			}
		}

		if ( %index >= %length )
		{
			return -1;
		}

		%next = getSubStr( %string, %index, 1 );
	}

	if ( %next $= "]" )
	{
		%object = new scriptObject()
		{
			class = "JSArray";
		};

		return %object TAB %index + 1;
	}

	while ( true )
	{
		%value = json_scan_once( %string, %index, %fast );

		if ( %value $= -1 )
		{
			if ( isObject( %object ) )
			{
				%object.delete();
			}

			return -1;
		}

		if ( !isObject( %object ) )
		{
			%object = new scriptObject()
			{
				class = "JSArray";
			};
		}

		%object.append( getField( %value, 0 ) );

		%index = getField( %value, 1 );
		%next = getSubStr( %string, %index, 1 );

		if ( strPos( " \t\n\r", %next ) >= 0 )
		{
			for ( %index ; %index < %length ; %index++ )
			{
				if ( strPos( " \t\n\r", getSubStr( %string, %index, 1 ) ) < 0 )
				{
					break;
				}
			}

			if ( %index >= %length )
			{
				%object.delete();
				return -1;
			}

			%next = getSubStr( %string, %index, 1 );
		}

		if ( %next $= "]" )
		{
			break;
		}

		if ( %next !$= "," )
		{
			%object.delete();
			return -1;
		}

		if ( %index >= %length )
		{
			%object.delete();
			return -1;
		}

		%index++;
		%next = getSubStr( %string, %index, 1 );

		if ( strPos( " \t\n\r", %next ) >= 0 )
		{
			for ( %index ; %index < %length ; %index++ )
			{
				if ( strPos( " \t\n\r", getSubStr( %string, %index, 1 ) ) < 0 )
				{
					break;
				}
			}

			if ( %index >= %length )
			{
				%object.delete();
				return -1;
			}
		}
	}

	return %object TAB %index + 1;
}

function json_scan_once( %string, %index, %fast )
{
	%char = getSubStr( %string, %index, 1 );

	if ( %char $= "\"" )
	{
		return json_parse_string( %string, %index + 1, %fast );
	}

	if ( %char $= "{" )
	{
		return json_parse_object( %string, %index + 1, %fast );
	}

	if ( %char $= "[" )
	{
		return json_parse_array( %string, %index + 1, %fast );
	}

	if ( %char $= "n" && getSubStr( %string, %index, 4 ) $= "null" )
	{
		if ( %fast )
		{
			return "" TAB %index + 4;
		}
		else
		{
			if ( !isObject( %obj = nameToID( "_JSNull" ) ) )
			{
				%obj = new scriptObject( "_JSNull" )
				{
					class = "JSNull";
				};
			}

			return %obj TAB %index + 4;
		}
	}

	if ( %char $= "t" && getSubStr( %string, %index, 4 ) $= "true" )
	{
		if ( %fast )
		{
			return 1 TAB %index + 4;
		}
		else
		{
			%obj = new scriptObject()
			{
				class = "JSBool";
				value = 1;
			};

			return %obj TAB %index + 4;
		}
	}

	if ( %char $= "f" && getSubStr( %string, %index, 5 ) $= "false" )
	{
		if ( %fast )
		{
			return 0 TAB %index + 5;
		}
		else
		{
			%obj = new scriptObject()
			{
				class = "JSBool";
				value = 0;
			};

			return %obj TAB %index + 5;
		}
	}

	%match = json_match_number( %string, %index );
	%length = strLen( %match );

	if ( %length )
	{
		if ( %match $= -1 )
		{
			return -1;
		}

		if ( %fast )
		{
			return %match TAB %index + %length;
		}
		else
		{
			%obj = new scriptObject()
			{
				class = "JSNumber";
				value = %match;
			};

			return %obj TAB %index + %length;
		}
	}

	return -1;
}

function json_serialize_object( %object )
{
	%json = "{";

	for ( %i = 0 ; %i < %object.count ; %i++ )
	{
		%json = %json @ "\"" @ expandEscape( %object.key[ %i ] ) @ "\"";
		%json = %json @ ":";
		%json = %json @ json_serialize( %object.value[ %object.key[ %i ] ] );

		if ( %i != %object.count - 1 )
		{
			%json = %json @ ",";
		}
	}

	return %json @ "}";
}

function json_serialize_array( %object )
{
	%json = "[";

	for ( %i = 0 ; %i < %object.count ; %i++ )
	{
		%json = %json @ json_serialize( %object.value[ %i ] );

		if ( %i != %object.count - 1 )
		{
			%json = %json @ ",";
		}
	}

	return %json @ "]";
}

function json_serialize( %data )
{
	if ( %data.class $= "JSObject" )
	{
		return json_serialize_object( %data );
	}
	else if ( %data.class $= "JSArray" )
	{
		return json_serialize_array( %data );
	}
	else if ( %data.class $= "JSString" )
	{
		return "\"" @ expandEscape( %data.value ) @ "\"";
	}
	else if ( %data.class $= "JSNumber" )
	{
		return %data.value;
	}
	else if ( %data.class $= "JSBool" )
	{
		return ( %data.value ? "true" : "false" );
	}
	else if ( %data.class $= "JSNull" )
	{
		return "null";
	}
	else if ( !strLen( %data ) )
	{
		return "null";
	}
	else if ( strLen( json_match_number( %data, 0 ) ) )
	{
		return %data;
	}
	else
	{
		return "\"" @ expandEscape( %data ) @ "\"";
	}
}

function json_dump( %data, %file )
{
	%json = json_dumps( %data );

	if ( %json $= -1 )
	{
		error( "json_dump() - failed to serialize data" );
		return -1;
	}

	if ( !isWriteableFileName( %file ) )
	{
		error( "json_dump() - file is not a writeable file name" );
		return -1;
	}

	%fo = new fileObject();
	%fo.openForWrite( %file );
	%fo.writeLine( %json );
	%fo.close();
	%fo.delete();

	return %json;
}

function json_dumps( %data )
{
	return json_serialize( %data );
}

function json_load( %file, %fast )
{
	if ( !isFile( %file ) )
	{
		return -1;
	}

	%fo = new fileObject();
	%fo.openForRead( %file );

	while ( !%fo.isEOF() )
	{
		%json = %json @ %fo.readLine() @ "\n";
	}

	%fo.close();
	%fo.delete();

	return json_loads( %json, %fast );
}

function json_loads( %json, %fast )
{
	return getField( json_scan_once( trim( %json ), 0, %fast ), 0 );
}