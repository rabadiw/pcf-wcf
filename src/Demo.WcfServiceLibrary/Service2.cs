using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Demo.WcfServiceLibrary
{
  // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service2" in both code and config file together.
  public class Service2 : IService2
  {
    public string GetSecureData(int value) => $"You securely entered: {value}";
  }
}
