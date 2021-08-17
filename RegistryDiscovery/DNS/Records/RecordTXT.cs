#region Using Namespaces

using System.Text;
using System.Collections.Generic;

#endregion

#region RFC Info

/*
3.3.14. TXT RDATA format

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    /                   TXT-DATA                    /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

TXT-DATA        One or more <character-string>s.

TXT RRs are used to hold descriptive text.  The semantics of the text
depends on the domain where it is found.
 * 
*/

#endregion

public class RecordTXT : Record
{
    #region Public Members

    public List<string> TXT;

    #endregion

    #region Constructors

    public RecordTXT(RecordReader rr, int Length)
	{
		int pos	= rr.Position;
		TXT		= new List<string>();
		
		while ((rr.Position - pos) < Length)
			TXT.Add(rr.ReadString());
	}

    #endregion

    #region Public Methods

    public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		
		foreach (string txt in TXT)
			sb.Append($"\"{txt}\" ");

		return sb.ToString().TrimEnd();
	}

    #endregion
}