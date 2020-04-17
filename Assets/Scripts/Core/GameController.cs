using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public bool PlayerVsAI = true;
    public string PlayerOneName = "You";
    public string PlayerTwoName = "Dummy AI";

    private PlayerOneAI playerOneAI;
    private PlayerTwoAI playerTwoAI;

    public GameObject playerOnePiece;
    public GameObject playerTwoPiece;

    public GameObject pieceboard;
    public GameObject winningText;

    string playerWonText = "Winner:";
    string drawText = "draw!";

    public GameObject btnPlayAgain;
    bool btnPlayAgainTouching = false;
    Color btnPlayAgainOrigColor;
    Color btnPlayAgainHoverColor = new Color(255, 143, 4);

    GameObject gameObjectboard;
    GameObject currentPiece;

    [HideInInspector]
    public static List<List<Piece>> board;

    bool isPlayerOneTurn = true;
    bool isLoading = true;
    bool isDropping = false;
    bool mouseButtonPressed = false;

    bool gameOver = false;
    bool isCheckingForWinner = false;

    void Start()
    {
        ConfigurePlayers();
        AdjustNumPiecesToWin();

        isLoading = true;
        Createboard();
        isLoading = false;
        CenterCamera();

        SetPlayerFirstTurnRandomly();

        winningText.SetActive(false);
        btnPlayAgain.SetActive(false);
        btnPlayAgainOrigColor = btnPlayAgain.GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLoading) return;

        if (isCheckingForWinner) return;

        if (gameOver)
        {
            GameOver();
            return;
        }

        if (currentPiece == null) currentPiece = SpawnPiece();

        if (isPlayerOneTurn && !playerOneAI)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPiece.transform.position = new Vector3(
                Mathf.Clamp(pos.x, 0, Config.numColumns - 1),
                gameObjectboard.transform.position.y + 1, 
                0
            );

            GetUserInput();
        }
        else
        {
            if (!isDropping) StartCoroutine(dropPiece(currentPiece));
        }
    }

    private void ConfigurePlayers()
    {
        if (!PlayerVsAI) playerOneAI = FindObjectsOfType<PlayerOneAI>()[0];
        playerTwoAI = FindObjectsOfType<PlayerTwoAI>()[0];
    }

    private void GetUserInput()
    {
        if (Input.GetMouseButtonDown(0) && !mouseButtonPressed && !isDropping)
        {
            mouseButtonPressed = true;
            StartCoroutine(dropPiece(currentPiece));
        }
        else
        {
            mouseButtonPressed = false;
        }
    }

    private void GameOver()
    {
        winningText.SetActive(true);
        btnPlayAgain.SetActive(true);

        UpdatePlayAgainButton();
    }

    private void SetPlayerFirstTurnRandomly()
    {
        isPlayerOneTurn = System.Convert.ToBoolean(Random.Range(0, 1));
    }

    private void AdjustNumPiecesToWin()
    {
        int max = Mathf.Max(Config.numRows, Config.numColumns);
        if (Config.numPiecesToWin > max) Config.numPiecesToWin = max;
    }

    private void CreateboardGameObject()
    {
        board = new List<List<Piece>>();
        gameObjectboard = GameObject.Find("board");
        if (gameObjectboard != null) DestroyImmediate(gameObjectboard);
        gameObjectboard = new GameObject("board");
    }

    void Createboard()
    {
        CreateboardGameObject();

        for (int x = 0; x < Config.numColumns; x++)
        {
            board.Add(new List<Piece>());
            for (int y = 0; y < Config.numRows; y++)
            {
                board[x].Add(Piece.Empty);
                Instantiate(pieceboard, new Vector3(x, y * -1, -1), Quaternion.identity, gameObjectboard.transform);
            }
        }
    }

    private void CenterCamera()
    {
        Camera.main.transform.position = new Vector3(
            (Config.numColumns - 1) / 2.0f, 
            -((Config.numRows - 1) / 2.0f),
            Camera.main.transform.position.z
        );

        winningText.transform.position = new Vector3(
            (Config.numColumns - 1) / 2.0f,
            -((Config.numRows - 1) / 2.0f) + 1,
            winningText.transform.position.z
        );

        btnPlayAgain.transform.position = new Vector3(
            (Config.numColumns - 1) / 2.0f,
            -((Config.numRows - 1) / 2.0f) - 1,
            btnPlayAgain.transform.position.z
        );
    }

    bool isAValidColumn(int column)
    {
        return column >= Config.numColumns ||
                column < 0 ||
                board[column][0] != Piece.Empty;
    }
	GameObject SpawnPiece()
	{
        Vector3 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (!isPlayerOneTurn) 
        {
            List<List<Piece>> temp = board;
            int spawnColumn = playerTwoAI.nextMove();
            if (isAValidColumn(spawnColumn)) spawnColumn = GetRandomFreeColumn();
            spawnPos = new Vector3(spawnColumn, 0, 0);
            board = temp;
        }

        if (isPlayerOneTurn && playerOneAI != null)
        {
            List<List<Piece>> temp = board;
            int spawnColumn = playerOneAI.nextMove();
            if (isAValidColumn(spawnColumn)) spawnColumn = GetRandomFreeColumn();
            spawnPos = new Vector3(spawnColumn, 0, 0);
            board = temp;
        }

		GameObject piece = Instantiate(
				isPlayerOneTurn ? playerOnePiece : playerTwoPiece,
				new Vector3(
				    Mathf.Clamp(spawnPos.x, 0, Config.numColumns -1), 
				    gameObjectboard.transform.position.y + 1, 0),
				    Quaternion.identity
                ) as GameObject;

		return piece;
	}

	void UpdatePlayAgainButton()
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
		if (Physics.Raycast(ray, out hit) && hit.collider.name == btnPlayAgain.name)
		{
			btnPlayAgain.GetComponent<Renderer>().material.color = btnPlayAgainHoverColor;
			if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && btnPlayAgainTouching == false)
			{
				btnPlayAgainTouching = true;
					
				//Createboard();
                SceneManager.LoadScene(0);
			}
		}
		else
		{
			btnPlayAgain.GetComponent<Renderer>().material.color = btnPlayAgainOrigColor;
		}
			
		if(Input.touchCount == 0) btnPlayAgainTouching = false;
	}

	IEnumerator dropPiece(GameObject gObject)
	{
		isDropping = true;

		Vector3 startPosition = gObject.transform.position;
		Vector3 endPosition = new Vector3();

		// round to a grid cell
		int x = Mathf.RoundToInt(startPosition.x);
		startPosition = new Vector3(x, startPosition.y, startPosition.z);

		// is there a free cell in the selected column?
		bool foundFreeCell = false;
		for(int i = Config.numRows -1; i >= 0; i--)
		{
			if(board[x][i] == 0)
			{
				foundFreeCell = true;
				board[x][i] = isPlayerOneTurn ? Piece.PlayerOne : Piece.PlayerTwo;
				endPosition = new Vector3(x, i * -1, startPosition.z);

				break;
			}
		}

		if(foundFreeCell)
		{
			// Instantiate a new Piece, disable the temporary
			GameObject newPiece = Instantiate (gObject) as GameObject;
			currentPiece.GetComponent<Renderer>().enabled = false;

			float distance = Vector3.Distance(startPosition, endPosition);

			float elapsedTime = 0;
			while(elapsedTime < 1)
			{
                elapsedTime += Time.deltaTime * Config.dropTime * ((Config.numRows - distance) + 1);

                newPiece.transform.position = Vector3.Lerp (startPosition, endPosition, elapsedTime);
				yield return null;
			}

            newPiece.transform.parent = gameObjectboard.transform;

			// remove the temporary gameobject
			DestroyImmediate(currentPiece);

			// run coroutine to check if someone has won
			StartCoroutine(Won());

			// wait until winning check is done
			while(isCheckingForWinner) yield return null;

			isPlayerOneTurn = !isPlayerOneTurn;
		}

		isDropping = false;
		yield return 0;
	}

	IEnumerator Won()
	{
		isCheckingForWinner = true;

		for(int x = 0; x < Config.numColumns; x++)
		{
			for(int y = 0; y < Config.numRows; y++)
			{
				// Get the Laymask to Raycast against, if its Players turn only include
				// Layermask Blue otherwise Layermask Red
				int layermask = isPlayerOneTurn ? (1 << 8) : (1 << 9);

				// If its Players turn ignore red as Starting piece and wise versa
				if(board[x][y] != (isPlayerOneTurn ? Piece.PlayerOne : Piece.PlayerTwo)) continue;

				// shoot a ray of length 'numPiecesToWin - 1' to the right to test horizontally
				RaycastHit[] hitsHorz = Physics.RaycastAll(
					new Vector3(x, y * -1, 0),
					Vector3.right,
                    Config.numPiecesToWin - 1,
					layermask
                );

				// return true (won) if enough hits
				if(hitsHorz.Length == Config.numPiecesToWin - 1)
				{
					gameOver = true;
					break;
				}

				// shoot a ray up to test vertically
				RaycastHit[] hitsVert = Physics.RaycastAll(
					new Vector3(x, y * -1, 0), 
					Vector3.up,
                    Config.numPiecesToWin - 1, 
					layermask);
					
				if(hitsVert.Length == Config.numPiecesToWin - 1)
				{
					gameOver = true;
					break;
				}

				// test diagonally
				if(Config.allowDiagonally)
				{
					// calculate the length of the ray to shoot diagonally
					float length = Vector2.Distance(new Vector2(0, 0), new Vector2(Config.numPiecesToWin - 1, Config.numPiecesToWin - 1));

					RaycastHit[] hitsDiaLeft = Physics.RaycastAll(
						new Vector3(x, y * -1, 0), 
						new Vector3(-1 , 1), 
						length, 
						layermask
                    );
						
					if(hitsDiaLeft.Length == Config.numPiecesToWin - 1)
					{
						gameOver = true;
						break;
					}

					RaycastHit[] hitsDiaRight = Physics.RaycastAll(
						new Vector3(x, y * -1, 0), 
						new Vector3(1 , 1), 
						length, 
						layermask);
						
					if(hitsDiaRight.Length == Config.numPiecesToWin - 1)
					{
						gameOver = true;
						break;
					}
				}

				yield return null;
			}

			yield return null;
		}

		// if Game Over update the winning text to show who has won
		if(gameOver == true)
		{
			winningText.GetComponent<TextMesh>().text = playerWonText + " " + (isPlayerOneTurn ? PlayerOneName : PlayerTwoName);
		}
		else 
		{
			// check if there are any empty cells left, if not set game over and update text to show a draw
			if(!boardContainsEmptyCell())
			{
				gameOver = true;
				winningText.GetComponent<TextMesh>().text = drawText;
			}
		}

		isCheckingForWinner = false;

		yield return 0;
	}

	bool boardContainsEmptyCell()
	{
		for(int x = 0; x < Config.numColumns; x++)
		{
			for(int y = 0; y < Config.numRows; y++)
			{
				if(board[x][y] == Piece.Empty)
					return true;
			}
		}
		return false;
	}

    public int GetRandomFreeColumn()
    {
        List<int> possibleMoves = new List<int>();
        for (int x = 0; x < Config.numColumns; x++)
            for (int y = 0; y < Config.numRows; y++)
                if (board[x][y] == Piece.Empty)  possibleMoves.Add(x);

        return possibleMoves[Random.Range(0, possibleMoves.Count)];
    }
}


