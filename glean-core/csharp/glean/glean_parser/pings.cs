using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace Glean.glean_parser
{
  public class Ping
  {
    public static readonly List<string> RESERVED_PING_NAMES = new List<string> { "baseline", "metrics", "events", "deletion_request" };

    public string name;
    public string description;
    public List<string> bugs = new List<string>();
    public List<string> notificationEmails = new List<string>();
    public List<string> dataReviews = null;
    public bool includeClientId = false;
    public bool sendIfEmpty = false;
    public Dictionary<string, string> reasons = null;
    public bool _validated = false;

    public Ping(string aName,
        string aDescription,
        List<string> aBugs,
        List<string> aNotificationEmails,
        List<string> aDataReviews = null,
        bool aIncludeClientId = false,
        bool aSendIfEmpty = false,
        Dictionary<string, string> aReasons = null,
        bool aValidated = false)
    {
      name = aName;
      description = aDescription;
      bugs = aBugs;
      notificationEmails = aNotificationEmails;
      dataReviews = aDataReviews;
      includeClientId = aIncludeClientId;
      sendIfEmpty = aSendIfEmpty;
      reasons = aReasons;
      _validated = aValidated;
    }

    public static Ping MakePing(string aName, YamlMappingNode aPingInfo)
    {
      var description = aPingInfo.Children.ContainsKey("description") ?
          aPingInfo["description"].ToString() : "";

      List<string> notification_emails = null;
      if (aPingInfo.Children.ContainsKey("notification_emails"))
      {
        var items = (YamlSequenceNode)aPingInfo.Children[new YamlScalarNode("notification_emails")];
        notification_emails = new List<string>();
        foreach (var item in items)
        {
          notification_emails.Add(((YamlScalarNode)item).Value);
        }
      }

      List<string> bugs = null;
      if (aPingInfo.Children.ContainsKey("bugs"))
      {
        var items = (YamlSequenceNode)aPingInfo.Children[new YamlScalarNode("bugs")];
        if (items != null)
        {
          bugs = new List<string>();
          foreach (var item in items)
          {
            bugs.Add(((YamlScalarNode)item).Value);
          }
        }
      }

      List<string> data_reviews = null;
      if (aPingInfo.Children.ContainsKey("data_reviews"))
      {
        var items = (YamlSequenceNode)aPingInfo.Children[new YamlScalarNode("data_reviews")];
      
        data_reviews = new List<string>();
        foreach (var item in items)
        {
          data_reviews.Add(((YamlScalarNode)item).Value);
        }
      }

      bool include_client_id = false;
      if (aPingInfo.Children.ContainsKey("include_client_id"))
      {
        include_client_id = aPingInfo["include_client_id"].ToString().Equals("true");
      }
      
      bool send_if_empty = false;
      if (aPingInfo.Children.ContainsKey("send_if_empty"))
      {
        send_if_empty = aPingInfo["send_if_empty"].ToString().Equals("true");
      }
      
      Dictionary<string, string> reasons = null;
      if (aPingInfo.Children.ContainsKey("reasons"))
      {
        var reasonItems = (YamlMappingNode)aPingInfo.Children[new YamlScalarNode("reasons")];
        reasons = new Dictionary<string, string>();
        foreach (var item in reasonItems.Children)
        {
          reasons[item.Key.ToString()] = item.Value.ToString();
        }
      }

      var ping = new Ping(aName, description, bugs, notification_emails, data_reviews,
        include_client_id, send_if_empty, reasons);
      return ping;
    }
  }
}
