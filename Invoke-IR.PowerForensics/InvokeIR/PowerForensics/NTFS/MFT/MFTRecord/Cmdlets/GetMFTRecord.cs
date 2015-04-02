﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace InvokeIR.PowerForensics.NTFS.MFT
{

    #region GetMFTRecordCommand
    /// <summary> 
    /// This class implements the Get-MFTRecord cmdlet. 
    /// </summary> 

    [Cmdlet(VerbsCommon.Get, "MFTRecord", DefaultParameterSetName = "Zero", SupportsShouldProcess = true)]
    public class GetMFTRecordCommand : PSCmdlet
    {

        #region Parameters

        /// <summary> 
        /// This parameter provides the Name of the Volume
        /// for which the FileRecord object should be
        /// returned.
        /// </summary> 

        [Parameter(Mandatory = true)]
        public string VolumeName
        {
            get { return volume; }
            set { volume = value; }
        }
        private string volume;

        /// <summary> 
        /// This parameter provides the MFTIndexNumber for the 
        /// FileRecord object that will be returned.
        /// </summary> 

        [Parameter(Mandatory = true, ParameterSetName = "Index")]
        public int IndexNumber
        {
            get { return indexNumber; }
            set { indexNumber = value; }
        }
        private int indexNumber;

        /// <summary> 
        /// This parameter provides the FileName for the 
        /// FileRecord object that will be returned.
        /// </summary> 

        [Parameter(Mandatory = true, ParameterSetName = "Path")]
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }
        private string filePath;

        /// <summary> 
        /// This parameter provides the FileName for the 
        /// FileRecord object that will be returned.
        /// </summary> 

        [Parameter(Mandatory = false, ParameterSetName = "Path")]
        [Parameter(Mandatory = false, ParameterSetName = "Index")]
        public SwitchParameter AsBytes
        {
            get { return asbytes; }
            set { asbytes = value; }
        }
        private SwitchParameter asbytes;

        #endregion Parameters

        #region Cmdlet Overrides

        /// <summary> 
        /// The ProcessRecord instantiates a NTFSVolumeData objects that
        /// corresponds to the VolumeName that is specified.
        /// </summary> 

        protected override void ProcessRecord()
        {

            Regex lettersOnly = new Regex("^[a-zA-Z]{1}$");

            if (lettersOnly.IsMatch(volume))
            {

                volume = @"\\.\" + volume + ":";
                
            }

            string volLetter = volume.TrimStart('\\').TrimStart('.').TrimStart('\\') + '\\';

            WriteDebug("VolumeName: " + volume);

            byte[] mftBytes = MasterFileTable.GetBytes(volume);
            
            if(this.MyInvocation.BoundParameters.ContainsKey("FilePath"))
            {
                
                int index = InvokeIR.PowerForensics.NTFS.MFT.IndexNumber.Get(volume, filePath);
                
                if (asbytes)
                {
                    WriteObject(MFTRecord.getMFTRecordBytes(mftBytes, index));
                }

                else
                {
                    WriteObject(MFTRecord.Get(mftBytes, index, volLetter, filePath));
                }
            }

            else if(this.MyInvocation.BoundParameters.ContainsKey("IndexNumber"))
            {
                if (asbytes)
                {
                    WriteObject(MFTRecord.getMFTRecordBytes(mftBytes, indexNumber));
                }

                else
                {
                    WriteObject(MFTRecord.Get(mftBytes, indexNumber, volLetter, null));
                }
            }

            else
            {
                MFTRecord[] records = MFTRecord.GetInstances(mftBytes, volLetter);
                
                foreach(MFTRecord record in records)
                {
                    WriteObject(record);
                }
            }
            

        } // ProcessRecord 

        #endregion Cmdlet Overrides

    } // End GetFSstatCommand class. 

    #endregion GetMFTRecordCommand

}