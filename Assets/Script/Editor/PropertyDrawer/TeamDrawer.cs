#if Temp
using System.Linq;
using PixelCollector.Worlds;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Team))]
public class TeamDrawer : PropertyDrawer
{
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    EditorGUI.BeginProperty(position, label, property);

    var selectedTeam = property.Get<Team>();

    var index = EditorGUI.Popup(EditorGUILayout.GetControlRect(), "Team", selectedTeam.Id,
      (from team in Team.ActiveTeams select team.Name).ToArray());
    selectedTeam = Team.ActiveTeams[index];
    property.Set(selectedTeam);

    EditorGUI.EndProperty();
  }
}

#endif