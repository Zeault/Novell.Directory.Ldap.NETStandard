﻿/******************************************************************************
* The MIT License
* Copyright (c) 2003 Novell Inc.  www.novell.com
*
* Permission is hereby granted, free of charge, to any person obtaining  a copy
* of this software and associated documentation files (the Software), to deal
* in the Software without restriction, including  without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to  permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*******************************************************************************/

using Novell.Directory.Ldap.Asn1;
using System.IO;

namespace Novell.Directory.Ldap.Controls
{
    /// <summary>
    ///     LdapEntryChangeControl is a Server Control returned at the request
    ///     of the client in response to a persistent search request. It
    ///     contains additional information about a change such as what type of
    ///     change occurred.
    /// </summary>
    public class LdapEntryChangeControl : LdapControl
    {
        /// <summary>
        ///     This constructor is called by the SDK to create an
        ///     LdapEntryChangeControl. This constructor should NOT be called by
        ///     application developers. It must be public since it resides in a
        ///     different package than the classes that call it.
        ///     The Entry Change Control is defined as follows:
        ///     EntryChangeNotification ::= SEQUENCE {
        ///     changeType ENUMERATED {
        ///     add             (1),
        ///     delete          (2),
        ///     modify          (4),
        ///     modDN           (8)
        ///     },
        ///     previousDN   LdapDN OPTIONAL,     -- modifyDN ops. only
        ///     changeNumber INTEGER OPTIONAL     -- if supported
        ///     }.
        /// </summary>
        /// <param name="oid">
        ///     The OID of the control, as a dotted string.
        /// </param>
        /// <param name="critical">
        ///     True if the Ldap operation should be discarded if
        ///     the control is not supported. False if
        ///     the operation can be processed without the control.
        /// </param>
        /// <param name="value">
        ///     The control-specific data.
        /// </param>
        public LdapEntryChangeControl(string oid, bool critical, byte[] value)
            : base(oid, critical, value)
        {
            // Create a decoder objet
            var decoder = new LberDecoder();
            if (decoder == null)
            {
                throw new IOException("Decoding error.");
            }

            // We should get a sequence initially
            var asnObj = decoder.Decode(value);

            if (asnObj is not Asn1Sequence sequence)
            {
                throw new IOException("Decoding error.");
            }

            // The first element in the sequence should be an enumerated type
            var asn1Obj = sequence[0];
            if (asn1Obj is not Asn1Enumerated enumerated)
            {
                throw new IOException("Decoding error.");
            }

            ChangeType = enumerated.IntValue();

            // check for optional elements
            // 8 means modifyDN
            if (sequence.Count > 1 && ChangeType == 8)
            {
                // get the previous DN - it is encoded as an octet string
                asn1Obj = sequence[1];
                if (asn1Obj is not Asn1OctetString octetString)
                {
                    throw new IOException("Decoding error get previous DN");
                }

                PreviousDn = octetString.StringValue();
            }
            else
            {
                PreviousDn = string.Empty;
            }

            // check for change number
            if (sequence.Count == 3)
            {
                asn1Obj = sequence[2];
                if (asn1Obj is not Asn1Integer integer)
                {
                    throw new IOException("Decoding error getting change number");
                }

                ChangeNumber = integer.IntValue();
                HasChangeNumber = true;
            }
            else
            {
                HasChangeNumber = false;
            }
        }

        /// <summary>
        ///     returns the record number of the change in the servers change log.
        /// </summary>
        /// <returns>
        ///     the record number of the change in the server's change log.
        ///     The server may not return a change number. In this case the return
        ///     value is -1.
        /// </returns>
        public bool HasChangeNumber { get; }

        /// <summary>
        ///     returns the record number of the change in the servers change log.
        /// </summary>
        /// <returns>
        ///     the record number of the change in the server's change log.
        ///     The server may not return a change number. In this case the return
        ///     value is -1.
        /// </returns>
        public int ChangeNumber { get; }

        /// <summary>
        ///     Returns the type of change that occured.
        /// </summary>
        /// <returns>
        ///     returns one of the following values indicating the type of
        ///     change that occurred:
        ///     LdapPersistSearchControl.ADD
        ///     LdapPersistSearchControl.DELETE
        ///     LdapPersistSearchControl.MODIFY
        ///     LdapPersistSearchControl.MODDN.
        /// </returns>
        public int ChangeType { get; }

        /// <summary>
        ///     Returns the previous DN of the entry, if it was renamed.
        /// </summary>
        /// <returns>
        ///     the previous DN of the entry if the entry was renamed (ie. if the
        ///     change type is LdapersistSearchControl.MODDN.
        /// </returns>
        public string PreviousDn { get; }
    } // end class LdapEntryChangeControl
}
