window.god = window.god || (window.god = {});
(god.CSharp || (god.CSharp = {})).Me = {
  "I": [
    {
      "Name": "WakeUp",
      "Key": "1167702007.2093240825",
      "Parameters": [
        "name",
        "password"
      ],
      "Return": ""
    },
    {
      "Name": "Sleep",
      "Key": "1167702007.764117084"
    },
    {
      "Name": "Pay",
      "Key": "1167702007.1614899325",
      "Parameters": [
        "planId",
        "content"
      ]
    },
    {
      "Name": "Desire",
      "Key": "1167702007.314836140",
      "Parameters": [
        "thing"
      ],
      "Return": 0
    },
    {
      "Name": "GiveUp",
      "Key": "1167702007.1297976969",
      "Parameters": [
        "planId"
      ]
    },
    {
      "Name": "Finish",
      "Key": "1167702007.347262110",
      "Parameters": [
        "planId"
      ]
    },
    {
      "Name": "Forget",
      "Key": "1167702007.1429488596",
      "Parameters": [
        "planId"
      ]
    },
    {
      "Name": "QueryPlans",
      "Key": "1167702007.1806772885",
      "Parameters": [
        "start",
        "end",
        "skip",
        "take"
      ],
      "Return": [
        {
          "GodId": 0,
          "Done": false,
          "Abandoned": false,
          "God": null,
          "Efforts": [],
          "Content": null,
          "Id": 0,
          "AppearTime": new Date(
            1532073775461
          )
        }
      ]
    },
    {
      "Name": "QueryEfforts",
      "Key": "1167702007.2029403369",
      "Parameters": [
        "planId",
        "start",
        "end"
      ],
      "Return": [
        {
          "PlanId": 0,
          "Plan": null,
          "Content": null,
          "Id": 0,
          "AppearTime": new Date(
            1532073775502
          )
        }
      ]
    }
  ]
};