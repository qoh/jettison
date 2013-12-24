// JettisonCircuit JettisonCircuit(TCPObject socket)

function JettisonCircuit(%socket)
{
	return new ScriptObject()
	{
		class = JettisonCircuit;
		socket = %socket;
	};
}

// JettisonCircuit::send(string channel, data, [bool keep])

function JettisonCircuit::send(%this, %channel, %data)
{
	if (%channel $= "")
	{
		error("ERROR: Channel name cannot be blank!");
		return;
	}

	if (%data $= "")
	{
		%data = "{}";
	}

	if (!isJSONObject(%data))
	{
		%data = parseJSON(%data);
		%created = 1;
	}

	if (getJSONType(%data) !$= "hash")
	{
		if (%created)
		{
			%data.delete();
		}

		error("ERROR: Data to send is not a JSON hash!");
		return;
	}

	%data.set("type", %channel);
	%this.socket.send(dumpJSON(%data) @ "\r\n" );

	if (%created)
	{
		%data.delete();
	}
}

// JettisonCircuit::process(string line)

function JettisonCircuit::process(%this, %line)
{
	%data = parseJSON(%line);
	%type = getJSONType(%data);

	if (%type !$= "hash" || !%data.isKey("type"))
	{
		if (isJSONObject(%data))
		{
			%data.delete();
		}
	}

	%keep = %this._react(%data.get("type"), %data);

	if (!%keep)
	{
		%data.delete();
	}
}

// bool JettisonCircuit::subscribe(string channel, [SimObject object], string callback)

function JettisonCircuit::subscribe(%this, %channel, %object, %callback)
{
	if (%channel $= "")
	{
		error("ERROR: Channel name cannot be blank!");
		return 0;
	}

	if (%callback $= "")
	{
		if (%object $= "")
		{
			error("ERROR: Missing callback name!");
			return 0;
		}

		%callback = %object;
		%object = "";
	}

	if (%object !$= "")
	{
		if (!isObject(%object))
		{
			error("ERROR: Callback object does not exist!");
			return 0;
		}

		if (mFloatLength(%object, 0) !$= %object && !validateEvalName(%object))
		{
			error("ERROR: Callback object is not safe for eval!" );
			return 0;
		}

		if (!validateEvalName(%callback))
		{
			error("ERROR: Callback function is not safe for eval!" );
			return 0;
		}

		%exists = isFunction(%object.getClassName(), %callback) ||
				  isFunction(%object.getName(), %callback) ||
				  isFunction(%object.class, %callback);
	}
	else
	{
		if (!validateEvalName(%callback))
		{
			error("ERROR: Callback function is not safe for eval!" );
			return 0;
		}

		%exists = isFunction(%calback);
	}

	if (!%exists)
	{
		error("ERROR: Callback function does not exist!");
		return 0;
	}

	// %this.subscription's %channel and %i are reversed in case %channel contains a _ character.

	for (%i = 0; %i < %this.subscriptions[%channel]; %i++)
	{
		if (%this.subscription[%i, %channel] $= %object SPC %callback)
		{
			return 1;
		}
	}

	%this.subscription[%i, %channel] = %object SPC %callback;
	%this.subscriptions[%channel]++;

	return 1;
}

// bool JettisonCircuit::unsubscribe(string channel, [SimObject object], string callback)

function JettisonCircuit::unsubscribe(%this, %channel, %object, %callback)
{
	for (%i = 0; %i < %this.subscriptions[%channel]; %i++)
	{
		if (%found)
		{
			%this.subscription[%i, %channel] = %this.subscription[%i + 1, %channel];
		}
		else if (%this.subscription[%i, %channel] $= %object SPC %callback)
		{
			%found = true;
			%i--;
		}
	}

	if (!%found)
	{
		return 0;
	}

	%this.subscription[%this.subscriptions[%channel]--, %channel] = "";
	return 1;
}

function JettisonCircuit::_react(%this, %channel, %data)
{
	for (%i = 0 ; %i < %this.subscriptions[%channel]; %i++)
	{
		%object = getWord(%this.subscription[%i, %channel], 0);
		%callback = getWord(%this.subscription[%i, %channel], 1);

		if (%object !$= "" && !isObject(%object))
		{
			%this.unsubscribe(%channel, %object, %callback);
			%i--;

			continue;
		}

		if (%object !$= "")
		{
			eval("%keep=%keep||" @ %object @ "." @ %callback @ "(%data,%channel);");
		}
		else
		{
			%keep = %keep || call(%callback, %data, %channel);
		}
	}

	return %keep;
}

// validateEvalName
//  @private

function validateEvalName(%val)
{
	%all = "abcdefghijklmnopqrstuvwxyz0123456789_";
	%fst = "abcdefghijklmnopqrstuvwxyz_";

	%len = strLen(%val);

	for (%i = 0; %i < %len; %i++)
	{
		%chr = getSubStr(%val, %i, 1);

		if (striPos(%i ? %all : %fst, %chr) == -1)
		{
			return 0;
		}
	}

	return %i != 0;
}