namespace FirePiercerCommon
{
    public enum PierceHeader : short
    {
        /// <summary>
        /// Don't use
        /// </summary>
        Invalid = 0x00,

        /// <summary>
        /// Reach out hand, payload is 4 bytes with version
        /// </summary>
        Handshake = 0x01,
        
        /// <summary>
        /// Hand accepted, payload is uint32 with unique client id
        /// </summary>
        HandshakeOK = 0x02,

        /// <summary>
        /// A UTF-16 text message for the other end
        /// </summary>
        Message = 0x03,

        /// <summary>
        /// Screenshot
        /// </summary>
        ScreenShot = 0x04,

        /// <summary>
        /// Initiates, changes or stops a remote desk session
        /// </summary>
        RemoteDeskRequest = 0x05,
        
        Socks5 = 0x06,

        /// <summary>
        /// Used for testing
        /// </summary>
        RoundTrip = 0x07
    }
}