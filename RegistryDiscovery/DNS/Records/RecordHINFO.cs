#region Using Namespaces

using System;

#endregion

#region RFC Info

/*
 3.3.2. HINFO RDATA format

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    /                      CPU                      /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    /                       OS                      /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

CPU             A <character-string> which specifies the CPU type.

OS              A <character-string> which specifies the operating
                system type.

Standard values for CPU and OS can be found in [RFC-1010].

HINFO records are used to acquire general information about a host.  The
main use is for protocols such as FTP that can use special procedures
when talking between machines or operating systems of the same type.
 */

#endregion

public class RecordHINFO : Record
{
	#region Public Members

	public string OS;
	public string CPU;

    #endregion

    #region Constructors

    public RecordHINFO(RecordReader rr)
	{
		CPU = rr.ReadString();
		OS  = rr.ReadString();
	}

    #endregion

    #region Public Methods

    public override string ToString()
	{
		return $"CPU={CPU} OS={OS}";
	}

    #endregion
}