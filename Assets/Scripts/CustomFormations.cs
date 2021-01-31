using UnityEngine;
using System;
using System.Collections.Generic;

//My current solution is [HideInInspector] [SerializeField] but y doesnt it work without the serialize tag tho

public class CustomFormations : MonoBehaviour
{
    public enum Mode { Random, Sequence };
    public Mode mode;

    [HideInInspector] [SerializeField] private List<Formation> formations; //If this is not serialized its data will reset on play
    [HideInInspector] public List<MemberData> memberData = new List<MemberData>();
    [HideInInspector] public List<int> openedFormations = new List<int>();
    [HideInInspector] public float tickPeriod;
    [HideInInspector] public bool editModeEnabled;
    [SerializeField] private List<FormationSpawnDetails> formationSpawnRates;
    public List<SequenceData> waveSequence;
    
    private List<int> candidates;
    private EnemySpawner enemySpawner;
    private Transform player;
    private Action notifyLastWave;
    private float x = 0;
    private int next;

    private void Awake()
    {
        if (editModeEnabled)
            Debug.Log("You forgot to save the formations. Object name: " + name);
    }

    private void Start()
    {
        candidates = new List<int>();
        enemySpawner = GameObject.Find("Level Designer").GetComponent<EnemySpawner>();
        player = GameObject.Find("Player").transform;
        if (mode == Mode.Random)
            next = -1;
        else
            next = 0;
    }

    private void SpawnRandomCandidate()
    {
        if (candidates.Count == 0)
            return;
        int index = UnityEngine.Random.Range(0, candidates.Count);
        List<Member> spawnees = formations[candidates[index]].GetMembers();
        Vector2 range = formationSpawnRates[candidates[index]].xRange;
        float xCor = UnityEngine.Random.Range(range.x, range.y);
        Vector3 basePosition = new Vector3(xCor, 1.69f, player.position.z + 40);
        foreach (Member spawnee in spawnees)
        {
            enemySpawner.Spawn(spawnee.enemyIndex, basePosition + spawnee.position);
        }
        candidates.Clear();
    }

    private void Update()
    {
        if (formations == null || formations.Count == 0)
            return;
        if (mode == Mode.Sequence)
        {
            if (next == waveSequence.Count)
            {
                notifyLastWave();
                next = 0;
            }
            return;
        }
        if (enemySpawner.bossMode)
            return;
        SpawnRandomCandidate();
        x += Time.deltaTime;
        if (x > tickPeriod)
            x = UnityEngine.Random.value;
        else
            return;
        for (int i = 0; i < formationSpawnRates.Count; i++)
            if (x < formationSpawnRates[i].spawnRate)
                candidates.Add(i);
        x = 0;
    }

    public void NextFormation(Action notifyLastWave)
    {
        if (mode == Mode.Random)
            return;
        this.notifyLastWave = notifyLastWave;
        List<Member> spawnees = formations[waveSequence[next].formationIndex].GetMembers();
        Vector3 basePosition = player.position + waveSequence[next].relativePosition;
        foreach (Member spawnee in spawnees)
        {
            enemySpawner.Spawn(spawnee.enemyIndex, basePosition + spawnee.position);
        }
        next++;
    }

    //Editor Code
    public void ConnectToSpawner()
    {
        if (mode == Mode.Sequence)
            return;
        if (editModeEnabled)
        {
            Debug.Log("Cannot connect to Spawner when in edit mode");
            return;
        }
        if (formationSpawnRates == null)
            formationSpawnRates = new List<FormationSpawnDetails>();
        formationSpawnRates.Clear();
        for (int i = 0; i < formations.Count; i++)
        {
            FormationSpawnDetails temp = new FormationSpawnDetails(formations[i].name, formations[i].GetRange());
            formationSpawnRates.Add(temp);
        }
    }

    public string[] GetFormationNames()
    {
        if (formations == null)
            return null;
        string[] names = new string[formations.Count];
        for (int i = 0; i < names.Length; i++)
        {
            names[i] = formations[i].name;
        }

        return names;
    }

    public void ReviewFormations()
    {
        if (editModeEnabled)
            return;
        editModeEnabled = true;
    }

    public void Save()
    {
        if (!editModeEnabled)
            return;
        editModeEnabled = false;
        openedFormations.Clear();
        foreach (Formation formation in formations)
        {
            formation.SaveFormation();
        }
        for (int i = 0; i < formations.Count; i++)
        {
            if (formations[i].name == null)
            {
                formations.RemoveAt(i);
                i--;
            }
        }
    }

    public void UpdateFormations()
    {
        if (formations == null)
            return;
        foreach (int formationIndex in openedFormations)
        {
            formations[formationIndex].UpdateMemberRelativePositions();
        }
    }

    public void UpdateEnemyData()
    {
        memberData.Clear();
        EnemySpawner enemySpawner = GameObject.Find("Level Designer").GetComponent<EnemySpawner>();
        for (int i = 0; i < enemySpawner.enemyDetails.Count; i++)
        {
            MemberData temp = new MemberData(enemySpawner.enemyDetails[i].enemyName, 
                enemySpawner.enemyDetails[i].enemyPrefab.GetComponentInChildren<MeshRenderer>().sharedMaterial);
            memberData.Add(temp);
        }
    }

    public void EditFormation(int formationIndex)
    {
        if (!editModeEnabled)
        {
            Debug.Log("You must be in edit mode to edit a formation");
            return;
        }
        openedFormations.Add(formationIndex);
        GameObject formationObject = new GameObject(formations[formationIndex].name);
        formationObject.transform.parent = transform;
        formations[formationIndex].EnableEditMode(memberData, formationObject.transform);
    }

    public void SaveFormation(int formationIndex)
    {
        formations[formationIndex].SaveFormation();
        if (formations[formationIndex].name == null)
        {
            formations.RemoveAt(formationIndex);
        }
        openedFormations.Remove(formationIndex);
    }

    public void AddFormation(string name)
    {
        if (name == null)
            return;
        if (!editModeEnabled)
        {
            Debug.Log("You must be in edit mode to add a formation");
            return;
        }
        if (formations == null)
            formations = new List<Formation>();
        Formation formation = new Formation(name);
        formations.Add(formation);
        EditFormation(formations.Count - 1);
    }

    public void RenameFormation(int formationIndex, string newName)
    {
        if (!editModeEnabled)
        {
            Debug.Log("You must be in edit mode to rename a formation");
            return;
        }
        if (!openedFormations.Contains(formationIndex))
        {
            formations[formationIndex].name = newName;
            return;
        }
        GameObject currentChild;
        for (int i = 0; i < transform.childCount; i++)
        {
            currentChild = transform.GetChild(i).gameObject;
            if (currentChild.name == formations[formationIndex].name)
            {
                currentChild.name = newName;
                i = transform.childCount;
            }
        }
        formations[formationIndex].name = newName;
    }

    public void AddMember(int formationIndex, int enemyIndex)
    {
        if (!openedFormations.Contains(formationIndex))
        {
            Debug.Log("You can only add members to a formation you are editing");
            return;
        }
        formations[formationIndex].AddMember(memberData[enemyIndex].memberName, enemyIndex, memberData[enemyIndex].mat);
    }

    public void AddToSequence(int formationIndex)
    {
        SequenceData sd = new SequenceData(formationIndex, formations[formationIndex].GetRelativePosition());
        if (waveSequence == null)
            waveSequence = new List<SequenceData>();
        waveSequence.Add(sd);
    }

    [System.Serializable] public class MemberData
    {
        public string memberName;
        public Material mat;

        public MemberData(string memberName, Material mat)
        {
            this.memberName = memberName;
            this.mat = mat;
        }
    }

    [System.Serializable] public class Member
    {
        public string enemyName;
        public int enemyIndex;
        public Vector3 position;
        public GameObject dummy;

        public Member(string enemyName, int enemyIndex, Material mat, Transform myFormation)
        {
            this.enemyName = enemyName;
            this.enemyIndex = enemyIndex;
            dummy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            dummy.name = enemyName;
            dummy.transform.parent = myFormation;
            dummy.transform.localPosition = Vector3.zero;
            dummy.GetComponent<MeshRenderer>().sharedMaterial = mat;
        }
        public void UpdateRelativePos()
        {
            if (dummy == null)
            {
                enemyName = null;
                return;
            }
            position = dummy.transform.localPosition;
        }
        public void SavePos()
        {
            UpdateRelativePos();
            if (dummy != null)
                DestroyImmediate(dummy);
        }
        public void EnableEditMode(Material mat, Transform myFormation)
        {
            dummy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            dummy.name = enemyName;
            dummy.transform.parent = myFormation;
            dummy.transform.localPosition = position;
            dummy.GetComponent<MeshRenderer>().sharedMaterial = mat;
        }
    }

    [System.Serializable] public class Formation
    {
        public string name;
        [HideInInspector] [SerializeField] private List<Member> members; //if this is not serialized each formation will lose members on play
        private Transform me;
        [HideInInspector] [SerializeField] private bool isSaved; //if this is not serialized formations will be deleted if you save after playing once (clicking on play will nullify this variable and then saving will make a formation that is already saved save again) (which is a big no no for my code)

        private void DeleteFormation()
        {
            if (members != null)
            {
                foreach (Member member in members)
                {
                    member.SavePos();
                }
                members.Clear();
                members = null;
            }
            if (me != null)
                DestroyImmediate(me.gameObject);
        }
        public Formation(string name)
        {
            this.name = name;
            isSaved = false;
        }
        public void EnableEditMode(List<MemberData> mats, Transform me)
        {
            this.me = me;
            isSaved = false;
            if (members != null)
            {
                foreach (Member member in members)
                {
                    member.EnableEditMode(mats[member.enemyIndex].mat, me);
                }
            }
        }
        public void AddMember(string enemyName, int enemyIndex, Material mat)
        {
            if (members == null)
                members = new List<Member>();

            Member member = new Member(enemyName, enemyIndex, mat, me);
            members.Add(member);
        }
        public void UpdateMemberRelativePositions()
        {
            if (isSaved)
                return;
            if (members == null)
                return;
            foreach (Member member in members)
            {
                member.UpdateRelativePos();
            }
        }
        public void SaveFormation()
        {
            if (isSaved)
                return;
            if (me == null)
            {
                DeleteFormation();
                name = null;
                return;
            }
            if (members != null)
            {
                for (int i = 0; i < members.Count; i++)
                {
                    if (members[i].enemyName == null)
                    {
                        members.RemoveAt(i);
                        i--;
                    }
                }
                for (int i = 0; i < members.Count; i++)
                {
                    members[i].SavePos();
                }
            }
            DestroyImmediate(me.gameObject);
            isSaved = true;
        }
        public Vector2 GetRange()
        {
            if (members == null || members.Count < 1)
            {
                return Vector2.zero;
            }
            float minX = 0, maxX = 0;
            foreach (Member member in members)
            {
                if (member.position.x < minX)
                    minX = member.position.x;
                if (member.position.x > maxX)
                    maxX = member.position.x;
            }
            return new Vector2(-10f - minX, 10f - maxX);
        }
        public List<Member> GetMembers()
        {
            return members;
        }
        public Vector3 GetRelativePosition()
        {
            GameObject player = GameObject.Find("Player");
            if (player == null)
            {
                Debug.Log("Player not found, relative position will not be saved");
                return Vector3.zero;
            }
            return me.position - player.transform.position;
        }
    }

    [System.Serializable] public class FormationSpawnDetails
    {
        public string formationName;
        [Range(0, 1)] public float spawnRate;
        [HideInInspector] public Vector2 xRange;

        public FormationSpawnDetails(string formationName, Vector2 xRange)
        {
            this.formationName = formationName;
            this.xRange = xRange;
        }
    }

    [System.Serializable] public class SequenceData
    {
        public int formationIndex;
        public Vector3 relativePosition;

        public SequenceData(int formationIndex, Vector3 relativePosition)
        {
            this.formationIndex = formationIndex;
            this.relativePosition = relativePosition;
        }
    }
}
