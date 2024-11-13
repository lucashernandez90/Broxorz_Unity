using UnityEngine;
using System.Collections;

public class BlockController : MonoBehaviour
{
    public float rollDuration = 0.3f;  // Tempo de rotação
    private bool isRolling = false;    // Verifica se o bloco já está rolando

    private BoxCollider colliderDeitado;
    private BoxCollider colliderEmPe;

    void Start()
    {
        // Localiza e define os dois BoxColliders
        var colliders = GetComponents<BoxCollider>();

        if (colliders.Length >= 2)
        {
            colliderDeitado = colliders[0];
            colliderEmPe = colliders[1];

            // Ativa o colisor deitado inicialmente
            colliderDeitado.enabled = true;
            colliderEmPe.enabled = false;
        }
        else
        {
            Debug.LogError("Não foram encontrados dois BoxColliders no objeto do bloco. Certifique-se de adicionar dois BoxColliders ao objeto.");
        }
    }

    void Update()
    {
        if (!isRolling)
        {
            if (Input.GetKeyDown(KeyCode.W))
                StartCoroutine(Roll(Vector3.forward));
            else if (Input.GetKeyDown(KeyCode.S))
                StartCoroutine(Roll(Vector3.back));
            else if (Input.GetKeyDown(KeyCode.A))
                StartCoroutine(Roll(Vector3.left));
            else if (Input.GetKeyDown(KeyCode.D))
                StartCoroutine(Roll(Vector3.right));
        }
    }

    private IEnumerator Roll(Vector3 direction)
    {
        isRolling = true;

        Vector3 rotationPoint = transform.position + (Vector3.down + direction) * 0.5f;
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction);

        float elapsedTime = 0f;
        while (elapsedTime < rollDuration)
        {
            float angle = (90f / rollDuration) * Time.deltaTime;
            transform.RotateAround(rotationPoint, rotationAxis, angle);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ajusta a posição e rotação final para alinhar
        transform.position = new Vector3(
            Mathf.Round(transform.position.x),
            Mathf.Round(transform.position.y),
            Mathf.Round(transform.position.z)
        );
        transform.rotation = Quaternion.Euler(
            Mathf.Round(transform.rotation.eulerAngles.x / 90) * 90,
            Mathf.Round(transform.rotation.eulerAngles.y / 90) * 90,
            Mathf.Round(transform.rotation.eulerAngles.z / 90) * 90
        );

        // Verifica se o bloco está em pé e, se estiver, eleva levemente
        if (Mathf.Round(transform.eulerAngles.x) % 180 == 90 || Mathf.Round(transform.eulerAngles.z) % 180 == 90)
        {
            transform.position += Vector3.up * 0.1f; // Ajuste fino para evitar "afundamento"
        }

        UpdateColliders();
        isRolling = false;
    }


    private void UpdateColliders()
    {
        bool isStanding = Mathf.Round(transform.eulerAngles.x) % 180 == 90 || Mathf.Round(transform.eulerAngles.z) % 180 == 90;

        colliderDeitado.enabled = !isStanding;
        colliderEmPe.enabled = isStanding;
    }
}
