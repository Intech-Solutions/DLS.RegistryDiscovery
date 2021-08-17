#region Using Namespaces

using System;

#endregion

public class RecordEID : Record
{
    #region Public Members

    public byte[] RDATA;

    #endregion

    #region Constructors
    
	public RecordEID(RecordReader rr)
	{
		// Re-read length
		ushort RDLENGTH = rr.Readushort(-2);
		RDATA           = rr.ReadBytes(RDLENGTH);
	}

    #endregion

    #region Public Methods

    public override string ToString()
	{
		return "not-used";
	}

    #endregion
}