using System.Runtime.InteropServices;
using UnityEngine;

using MpCalculatorGraph = System.IntPtr;
using MpCalculatorGraphConfig = System.IntPtr;
using MpSidePacket = System.IntPtr;
using MpStatus = System.IntPtr;
using MpStatusOrPoller = System.IntPtr;
using ProtobufLogHandlerPtr = System.IntPtr;

namespace Mediapipe
{
  public class CalculatorGraph
  {
    private const string MediapipeLibrary = "mediapipe_c";

    public MpCalculatorGraph mpCalculatorGraph;

    static CalculatorGraph()
    {
      SetProtobufLogHandler(Marshal.GetFunctionPointerForDelegate(protobufLogHandler));
    }

    public CalculatorGraph(string configText)
    {
      this.mpCalculatorGraph = MpCalculatorGraphCreate();
      var status = Initialize(configText);

      if (status == null || !status.IsOk())
      {
        throw new System.SystemException(status == null ? "Failed to parse the config text" : status.ToString());
      }
    }

    ~CalculatorGraph() {
      MpCalculatorGraphDestroy(mpCalculatorGraph);
    }

    public Status StartRun(SidePacket sidePacket)
    {
      return new Status(MpCalculatorGraphStartRun(mpCalculatorGraph, sidePacket.GetPtr()));
    }

    public Status WaitUntilDone()
    {
      return new Status(MpCalculatorGraphWaitUntilDone(mpCalculatorGraph));
    }

    public StatusOrPoller AddOutputStreamPoller(string name)
    {
      return new StatusOrPoller(MpCalculatorGraphAddOutputStreamPoller(mpCalculatorGraph, name));
    }

    // TODO: add Packet, instead of StringPacket
    public Mediapipe.Status AddStringToInputStream(string name, string text, int timestamp)
    {
      return new Status(MpCalculatorGraphAddStringPacketToInputStream(mpCalculatorGraph, name, text, timestamp));
    }

    public Status CloseInputStream(string name)
    {
      return new Status(MpCalculatorGraphCloseInputStream(mpCalculatorGraph, name));
    }

    private Status Initialize(string configText)
    {
      var config = ParseMpCalculatorGraphConfig(configText);

      if (config == System.IntPtr.Zero)
      {
        Debug.Log("Failed to parse graph config");
        return null;
        // TODO: build error status
      }

      return new Status(MpCalculatorGraphInitialize(mpCalculatorGraph, config));
    }

    // Protobuf Logger
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    private delegate void ProtobufLogHandler(int level, string filename, int line, string message);
    private static readonly ProtobufLogHandler protobufLogHandler = LogProtobufMessage;
    private static ProtobufLogHandlerPtr protobufLogHandlerPtr;

    private static void LogProtobufMessage(int level, string filename, int line, string message)
    {
      Debug.Log($"[libprotobuf {FormatProtobufLogLevel(level)} {filename}:{line}] {message}");
    }

    private static string FormatProtobufLogLevel(int level)
    {
      switch (level)
      {
        case 1: return "WARNING";
        case 2: return "ERROR";
        case 3: return "FATAL";
        default: return "INFO";
      }
    }

    #region Externs

    // CalculatorGraph API
    [DllImport (MediapipeLibrary)]
    private static extern unsafe MpCalculatorGraph MpCalculatorGraphCreate();

    [DllImport (MediapipeLibrary)]
    private static extern unsafe void MpCalculatorGraphDestroy(MpCalculatorGraph graph);

    [DllImport (MediapipeLibrary)]
    private static extern unsafe System.IntPtr SetProtobufLogHandler([MarshalAs(UnmanagedType.FunctionPtr)]System.IntPtr debugCal);

    [DllImport (MediapipeLibrary)]
    private static extern unsafe MpCalculatorGraphConfig ParseMpCalculatorGraphConfig(string input);

    [DllImport (MediapipeLibrary)]
    private static extern unsafe MpStatus MpCalculatorGraphInitialize(MpCalculatorGraph graph, MpCalculatorGraphConfig config);

    [DllImport (MediapipeLibrary)]
    private static extern unsafe MpStatus MpCalculatorGraphStartRun(MpCalculatorGraph graph, MpSidePacket sidePacket);

    [DllImport (MediapipeLibrary)]
    private static extern unsafe MpStatus MpCalculatorGraphWaitUntilDone(MpCalculatorGraph graph);

    [DllImport (MediapipeLibrary)]
    private static extern unsafe MpStatusOrPoller MpCalculatorGraphAddOutputStreamPoller(MpCalculatorGraph graph, string name);

    [DllImport (MediapipeLibrary)]
    private static extern unsafe MpStatus MpCalculatorGraphAddStringPacketToInputStream(MpCalculatorGraph graph, string name, string packet, int timestamp);

    [DllImport (MediapipeLibrary)]
    private static extern unsafe MpStatus MpCalculatorGraphCloseInputStream(MpCalculatorGraph graph, string name);

    #endregion
  }
}
