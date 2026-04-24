using UnityEngine;
using TMPro;
using System.IO;

[System.Serializable]
public class Usuario
{
    public int id;
    public string nombre;
}

[System.Serializable]
public class ListaUsuarios
{
    public Usuario[] usuarios;
}

public class RegistroUsuarios : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField inputNombre;
    public TextMeshProUGUI listaUsuariosText;
    public GameObject panelUsuarios;

    private string ruta;
    private ListaUsuarios lista = new ListaUsuarios();

    void Start()
    {
        ruta = Application.persistentDataPath + "/usuarios.json";
        CargarUsuarios();
        panelUsuarios.SetActive(false);
    }

    // 🔹 Registrar usuario
    public void RegistrarUsuario()
    {
        string nombre = inputNombre.text.Trim();

        if (nombre == "") return;

        int nuevoId = (lista.usuarios != null) ? lista.usuarios.Length + 1 : 1;

        Usuario nuevo = new Usuario();
        nuevo.id = nuevoId;
        nuevo.nombre = nombre;

        if (lista.usuarios == null)
        {
            lista.usuarios = new Usuario[] { nuevo };
        }
        else
        {
            Usuario[] temp = new Usuario[lista.usuarios.Length + 1];

            for (int i = 0; i < lista.usuarios.Length; i++)
            {
                temp[i] = lista.usuarios[i];
            }

            temp[temp.Length - 1] = nuevo;
            lista.usuarios = temp;
        }

        GuardarUsuarios();
        inputNombre.text = "";
    }

    // 🔹 Guardar en JSON
    void GuardarUsuarios()
    {
        string json = JsonUtility.ToJson(lista, true);
        File.WriteAllText(ruta, json);
    }

    // 🔹 Cargar JSON
    void CargarUsuarios()
    {
        if (File.Exists(ruta))
        {
            string json = File.ReadAllText(ruta);
            lista = JsonUtility.FromJson<ListaUsuarios>(json);
        }
    }

    // 🔹 Mostrar usuarios en el panel
    public void MostrarUsuarios()
    {
        CargarUsuarios();

        listaUsuariosText.text = "";

        if (lista.usuarios == null || lista.usuarios.Length == 0)
        {
            listaUsuariosText.text = "No hay usuarios registrados";
        }
        else
        {
            foreach (Usuario u in lista.usuarios)
            {
                listaUsuariosText.text += "ID: " + u.id + " - " + u.nombre + "\n";
            }
        }

        panelUsuarios.SetActive(true);
    }

    // 🔹 Cerrar panel
    public void CerrarPanel()
    {
        panelUsuarios.SetActive(false);
    }
}