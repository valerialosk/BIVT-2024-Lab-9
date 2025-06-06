﻿using Lab_7;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Lab_9
{
    public class BlueXMLSerializer : BlueSerializer
    {
        public override string Extension { get { return "xml"; } }
        //Blue_1
        public class ResponseDTO
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public int Votes { get; set; }
            public string Surname { get; set; }
            public ResponseDTO() { }
            public ResponseDTO(Blue_1.Response response)
            {
                Type = response.GetType().Name;
                Name = response.Name;
                Votes = response.Votes;
                if (response is Blue_1.HumanResponse r) Surname = r.Surname;
            }
        }
        public override void SerializeBlue1Response(Blue_1.Response participant, string fileName)
        {
            if (participant == null || String.IsNullOrEmpty(fileName)) return;
            SelectFile(fileName);
            var dto = new ResponseDTO(participant);
            var xml = new XmlSerializer(typeof(ResponseDTO));
            using (var writer = new StreamWriter(FilePath)) //StreamWriter создает поток для записи в файл
            {
                xml.Serialize(writer, dto);
            }
        }
        public override Blue_1.Response DeserializeBlue1Response(string fileName)
        {
            if (String.IsNullOrEmpty(fileName)) return null;
            SelectFile(fileName);
            ResponseDTO dto;
            var xml = new XmlSerializer(typeof(ResponseDTO));
            Blue_1.Response result;
            using (var reader = new StreamReader(FilePath)) //StreamReader открывает файл для чтения
            {
                dto = (ResponseDTO)xml.Deserialize(reader);
            }
            if (dto.Surname != null)
            {
                result = new Blue_1.HumanResponse(dto.Name, dto.Surname, dto.Votes);
            }
            else
            {
                result = new Blue_1.Response(dto.Name, dto.Votes);
            }
            return result;
        }
        //Blue_2
        private static int[][] MatrixToZub(int[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int[][] m = new int[rows][];
            for (int i = 0; i < rows; i++)
            {
                m[i] = new int[cols];
                for (int j = 0; j < cols; j++)
                {
                    m[i][j] = matrix[i, j];
                }
            }
            return m;
        }
        private static int[,] ZubToMatrix(int[][] array)
        {
            int rows = array.Length;
            int cols = array[0].Length;
            int[,] matrix = new int[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = array[i][j];
                }
            }
            return matrix;
        }
        public class ParticipantBlue_2DTO
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public int[][] Marks { get; set; }
            public ParticipantBlue_2DTO() { }
            public ParticipantBlue_2DTO(Blue_2.Participant participant)
            {
                Type = participant.GetType().Name;
                Name = participant.Name;
                Surname = participant.Surname;
                Marks = MatrixToZub(participant.Marks); //преобразовываю в зубчатую матрицу
            }
        }
        public class WaterJumpDTO
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public int Bank { get; set; }
            public ParticipantBlue_2DTO[] Participants { get; set; }
            public WaterJumpDTO() { }
            public WaterJumpDTO(Blue_2.WaterJump jump)
            {
                Type = jump.GetType().Name;
                Name = jump.Name;
                Bank = jump.Bank;
                if (jump.Participants.Length > 0)
                {
                    Participants = new ParticipantBlue_2DTO[jump.Participants.Length];
                    for (int i = 0; i < jump.Participants.Length; i++)
                    {
                        Participants[i] = new ParticipantBlue_2DTO(jump.Participants[i]);
                    }
                }
            }
        }
        public override void SerializeBlue2WaterJump(Blue_2.WaterJump participant, string fileName)
        {
            if (participant == null || String.IsNullOrEmpty(fileName)) { return; }
            SelectFile(fileName);
            var dto = new WaterJumpDTO(participant);
            var xml = new XmlSerializer(typeof(WaterJumpDTO));
            using (var writer = new StreamWriter(FilePath))
            {
                xml.Serialize(writer, dto);
            }
        }
        public override Blue_2.WaterJump DeserializeBlue2WaterJump(string fileName)
        {
            if (String.IsNullOrEmpty(fileName)) return null;
            SelectFile(fileName);
            WaterJumpDTO dto;
            var xml = new XmlSerializer(typeof(WaterJumpDTO));
            Blue_2.WaterJump result;
            using (var reader = new StreamReader(FilePath)) 
            {
                dto = (WaterJumpDTO)xml.Deserialize(reader);
            }
            if (dto.Type == "WaterJump3m") result = new Blue_2.WaterJump3m(dto.Name, dto.Bank);
            else if (dto.Type == "WaterJump5m") result = new Blue_2.WaterJump5m(dto.Name, dto.Bank);
            else return null;
            if (dto.Participants != null)
            {
                for (int i = 0; i < dto.Participants.Length; i++)
                {
                    if (dto.Participants[i] == null) continue;
                    var participant = new Blue_2.Participant(dto.Participants[i].Name, dto.Participants[i].Surname);
                    int[,] marks = new int[2, 5];
                    if (dto.Participants[i].Marks == null) continue;
                    for (int j = 0; j < 2; j++)
                    {
                        if (j >= dto.Participants[i].Marks.Length || dto.Participants[i].Marks[j] == null)
                            continue;
                        for (int k = 0; k < 5; k++)
                        {
                            if (k >= dto.Participants[i].Marks[j].Length) break;
                            marks[j, k] = dto.Participants[i].Marks[j][k];
                        }
                    }
                    for (int j = 0; j < 2; j++)
                    {
                        int[] jump = new int[5]; //создаю массив, потому что в метод Jump можно добавлять только массив, а не матрицу
                        for (int k = 0; k < 5; k++)
                        {
                            jump[k] = marks[j, k];
                        }
                        participant.Jump(jump);
                    }
                    result.Add(participant);
                }
            }
            return result;
        }
        //Blue_3
        public class ParticipantBlue_3DTO
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public int[] Penalties { get; set; }
            public ParticipantBlue_3DTO() { }
            public ParticipantBlue_3DTO(Blue_3.Participant participant)
            {
                Type = participant.GetType().Name;
                Name = participant.Name;
                Surname = participant.Surname;
                if (participant.Penalties.Length > 0)
                {
                    Penalties = new int[participant.Penalties.Length];
                    for (int i = 0; i < participant.Penalties.Length; i++)
                    {
                        Penalties[i] = participant.Penalties[i];
                    }
                }
            }
        }
        public override void SerializeBlue3Participant<T>(T student, string fileName) //where T : Blue_3.Participant;
        {
            if (student == null || String.IsNullOrEmpty(fileName)) return;
            SelectFile(fileName);
            var dto = new ParticipantBlue_3DTO(student);
            var xml = new XmlSerializer(typeof(ParticipantBlue_3DTO));
            using (var writer = new StreamWriter(FilePath))
            {
                xml.Serialize(writer, dto);
            }
        }
        public override T DeserializeBlue3Participant<T>(string fileName) //where T : Blue_3.Participant
        {
            if (String.IsNullOrEmpty(fileName)) return null;
            SelectFile(fileName);
            ParticipantBlue_3DTO dto;
            var xml = new XmlSerializer(typeof(ParticipantBlue_3DTO));
            Blue_3.Participant result;
            using (var reader = new StreamReader(FilePath)) //StreamReader открывает файл для чтения
            {
                dto = (ParticipantBlue_3DTO)xml.Deserialize(reader);
            }
            if (dto.Type == "Participant") result = new Blue_3.Participant(dto.Name, dto.Surname);
            else if (dto.Type == "BasketballPlayer") result = new Blue_3.BasketballPlayer(dto.Name, dto.Surname);
            else if (dto.Type == "HockeyPlayer") result = new Blue_3.HockeyPlayer(dto.Name, dto.Surname);
            else return null;
            for (int i = 0; i < dto.Penalties.Length; i++)
            {
                result.PlayMatch(dto.Penalties[i]);
            }
            return (T)result;
        }
        //Blue_4
        public class TeamDTO
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public int[] Scores { get; set; }
            public TeamDTO() { }
            public TeamDTO(Blue_4.Team team)
            {
                if (team == null) return;
                Type = team.GetType().Name;
                Name = team.Name;
                Scores = new int[team.Scores.Length];
                if (team.Scores != null)
                {
                    for (int i = 0; i < team.Scores.Length; i++)
                    {
                        Scores[i] = team.Scores[i];
                    }
                }
            }
        }
        public class GroupBlue_4DTO
        {
            public string Name { get; set; }
            public TeamDTO[] ManTeams { get; set; }
            public TeamDTO[] WomanTeams { get; set; }
            public GroupBlue_4DTO() { }

            public GroupBlue_4DTO(Blue_4.Group group)
            {
                if (group == null) return;
                Name = group.Name;
                if (group.ManTeams != null && group.ManTeams.Length > 0)
                {
                    ManTeams = new TeamDTO[group.ManTeams.Length];
                    for (int i = 0; i < group.ManTeams.Length; i++)
                    {
                        ManTeams[i] = new TeamDTO(group.ManTeams[i]);
                    }
                }
                if (group.WomanTeams != null && group.WomanTeams.Length > 0)
                {
                    WomanTeams = new TeamDTO[group.WomanTeams.Length];
                    for (int i = 0; i < group.WomanTeams.Length; i++)
                    {
                        WomanTeams[i] = new TeamDTO(group.WomanTeams[i]);
                    }
                }
            }
        }
        public override void SerializeBlue4Group(Blue_4.Group participant, string fileName)
        {
            if (participant == null || String.IsNullOrEmpty(fileName)) return;
            SelectFile(fileName);
            var dto = new GroupBlue_4DTO(participant);
            var xml = new XmlSerializer(typeof(GroupBlue_4DTO));
            using (var writer = new StreamWriter(FilePath))
            {
                xml.Serialize(writer, dto);
            }
        }
        public override Blue_4.Group DeserializeBlue4Group(string fileName)
        {
            if (String.IsNullOrEmpty(fileName)) return null;
            SelectFile(fileName);
            GroupBlue_4DTO dto;
            var xml = new XmlSerializer(typeof(GroupBlue_4DTO));
            using (var reader = new StreamReader(FilePath)) //StreamReader открывает файл для чтения
            {
                dto = (GroupBlue_4DTO)xml.Deserialize(reader);
            }
            Blue_4.Group result = new Blue_4.Group(dto.Name);
            if (dto.ManTeams != null)
            {
                for (int i = 0; i < dto.ManTeams.Length; i++)
                {
                    if (dto.ManTeams[i] == null)
                    {
                        result.Add(default(Blue_4.ManTeam));
                        continue;
                    }
                    else if (dto.ManTeams[i].Type == "ManTeam")
                    {
                        Blue_4.ManTeam manTeam = new Blue_4.ManTeam(dto.ManTeams[i].Name);
                        if (dto.ManTeams[i].Scores != null)
                        {
                            for (int j = 0; j < dto.ManTeams[i].Scores.Length; j++)
                            {
                                manTeam.PlayMatch(dto.ManTeams[i].Scores[j]);
                            }
                        }
                        result.Add(manTeam);
                    }
                    else
                    {
                        result.Add(default(Blue_4.ManTeam));
                    }
                }
            }
            if (dto.WomanTeams != null)
            {
                for (int i = 0; i < dto.WomanTeams.Length; i++)
                {
                    if (dto.WomanTeams[i] == null)
                    {
                        result.Add(default(Blue_4.WomanTeam));
                        continue;
                    }
                    else if (dto.WomanTeams[i].Type == "WomanTeam")
                    {
                        Blue_4.WomanTeam womanTeam = new Blue_4.WomanTeam(dto.WomanTeams[i].Name);
                        if (dto.WomanTeams[i].Scores != null)
                        {
                            for (int j = 0; j < dto.WomanTeams[i].Scores.Length; j++)
                            {
                                womanTeam.PlayMatch(dto.WomanTeams[i].Scores[j]);
                            }
                        }
                        result.Add(womanTeam);
                    }
                    else result.Add(default(Blue_4.WomanTeam));
                }
            }
            return result;
        }
        //Blue_5
        public class SportsmanDTO
        {
            public string Name { get; set; }
            public string Surname { get; set; }
            public int Place { get; set; }
            public SportsmanDTO() { }
            public SportsmanDTO(Blue_5.Sportsman sportsman)
            {
                Name = sportsman.Name;
                Surname = sportsman.Surname;
                Place = sportsman.Place;
            }
        }
        public class TeamBlue_5DTO
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public SportsmanDTO[] Sportsmen { get; set; }
            public TeamBlue_5DTO() { }
            public TeamBlue_5DTO(Blue_5.Team team)
            {
                Type = team.GetType().Name;
                Name = team.Name;
                if (team.Sportsmen != null)
                {
                    Sportsmen = new SportsmanDTO[team.Sportsmen.Length];
                    for (int i = 0; i < team.Sportsmen.Length; i++)
                    {
                        if (team.Sportsmen[i] != null)
                        {
                            Sportsmen[i] = new SportsmanDTO(team.Sportsmen[i]);
                        }
                        else
                        {
                            Sportsmen[i] = default(SportsmanDTO);
                        }
                    }
                }
            }
        }
        public override void SerializeBlue5Team<T>(T group, string fileName) //where T : Blue_5.Team;
        {
            if (group == null || String.IsNullOrEmpty(fileName)) return;
            SelectFile(fileName);
            var dto = new TeamBlue_5DTO(group);
            var xml = new XmlSerializer(typeof(TeamBlue_5DTO));
            using (var writer = new StreamWriter(FilePath))
            {
                xml.Serialize(writer, dto);
            }
        }
        public override T DeserializeBlue5Team<T>(string fileName) //where T : Blue_5.Team;
        {
            if (String.IsNullOrEmpty(fileName)) return null;
            SelectFile(fileName);
            TeamBlue_5DTO dto;
            var xml = new XmlSerializer(typeof(TeamBlue_5DTO));
            using (var reader = new StreamReader(FilePath)) //StreamReader открывает файл для чтения
            {
                dto = (TeamBlue_5DTO)xml.Deserialize(reader);
            }
            T result;
            Blue_5.Team temp;
            if (dto.Type == "ManTeam")
            {
                temp = new Blue_5.ManTeam(dto.Name);
            }
            else if (dto.Type == "WomanTeam")
            {
                temp = new Blue_5.WomanTeam(dto.Name);
            }
            else return null;
            result = temp as T;
            if (dto.Sportsmen == null) return result;
            else
            {
                for (int i = 0; i < dto.Sportsmen.Length; i++)
                {
                    if (dto.Sportsmen[i] == null) continue;
                    var tempSportsman = new Blue_5.Sportsman(dto.Sportsmen[i].Name, dto.Sportsmen[i].Surname);
                    tempSportsman.SetPlace(dto.Sportsmen[i].Place);
                    temp.Add(tempSportsman);
                }
            }
            result = temp as T;
            return result;
        }
    }
}
