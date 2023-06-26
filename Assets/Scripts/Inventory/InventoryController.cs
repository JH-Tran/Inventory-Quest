using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [HideInInspector]
    private GridManager selectedItemGridManager;
    public GridManager SelectedItemGridManager { 
        get => selectedItemGridManager; 
        set { 
            selectedItemGridManager = value;
            inventoryHighlight.SetParent(value);
        } 
    }

    public GridManager dropGridManager;

    InventoryItem selectedItem;
    InventoryItem overlapItem;
    RectTransform rectTransform;

    [SerializeField] List<InventoryItemData> items;
    [SerializeField] InventoryItemData genericMovePrefab;

    [SerializeField] GameObject movePrefab;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;

    InventoryHighlight inventoryHighlight;

    Vector2Int oldPosition;
    InventoryItem itemToHighlight;


    private void Awake()
    {
        inventoryHighlight = GetComponent<InventoryHighlight>();
    }

    // Update is called once per frame
    void Update()
    {
        ItemIconDrag();

        /* if (Input.GetKeyDown(KeyCode.Q))
        {
            if (selectedItem == null)
            {
                CreateRandomItem();
            }
        }*/
        if (Input.GetKeyDown(KeyCode.W))
        {
            InsertRandomItem();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateItem();
        }

        if (selectedItemGridManager == null) {
            inventoryHighlight.Show(false);
            return; 
        }

        HandleHighlight();

        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseButtonPress();
        }
    }

    private void RotateItem()
    {
        if (selectedItem == null) { return; }

        selectedItem.Rotate();
    }
    private void InsertRandomItem()
    {
        if (dropGridManager == null) { return; }

        CreateRandomItem();
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        if (!InsertItemToDropGrid(itemToInsert))
        {
            Destroy(itemToInsert.gameObject);
        }
    }
    private bool InsertItemToDropGrid(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = dropGridManager.FindSpaceForObject(itemToInsert);
        if (posOnGrid == null) { return false;  }
        dropGridManager.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
        return true;
    }
    public void InsertPlayerAwakeMoves(UnitInstance playerUnitInstance, List<GameObject> moveGameObjectList)
    {
        for (int moveNum = 0; moveNum < moveGameObjectList.Count; moveNum++)
        {
            if (playerUnitInstance.moveList[moveNum] == null)
            {
                return;
            }
            //Create item gameobject and assign move information on the gameobject
            InventoryItem moveItem = CreateMoveItemGameObject();
            moveItem.GetComponent<MoveItem>().moveData = playerUnitInstance.moveList[moveNum];
            Vector2Int? posOnGrid = moveGameObjectList[moveNum].GetComponent<GridManager>().FindSpaceForObject(moveItem);
            if (posOnGrid == null) {
                Debug.LogError($"Move cannot be place on {moveGameObjectList[moveNum].name} because it is null.");
                return; 
            }
            moveGameObjectList[moveNum].GetComponent<GridManager>().PlaceItem(moveItem, posOnGrid.Value.x, posOnGrid.Value.y);
        }
    }
    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTileGridPosition();
        if (oldPosition == positionOnGrid) { return; }
        oldPosition = positionOnGrid;
        if (selectedItem == null)
        {
            itemToHighlight = selectedItemGridManager.GetItem(positionOnGrid.x, positionOnGrid.y);

            if (itemToHighlight != null)
            {
                inventoryHighlight.Show(true);
                inventoryHighlight.SetSize(itemToHighlight);
                inventoryHighlight.SetPosition(selectedItemGridManager, itemToHighlight);
            }
            else
            {
                inventoryHighlight.Show(false);
            }
        }
        else
        {
            inventoryHighlight.Show(selectedItemGridManager.BoundryCheck(positionOnGrid.x, positionOnGrid.y, selectedItem.WIDTH, selectedItem.HEIGHT));
            inventoryHighlight.SetSize(selectedItem);
            inventoryHighlight.SetPosition(selectedItemGridManager, selectedItem, positionOnGrid.x, positionOnGrid.y);
        }
    }
    private void CreateRandomItem()
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;
        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);

        int selectedItemId = UnityEngine.Random.Range(0, items.Count);
        inventoryItem.Set(items[selectedItemId]);
    }
    private InventoryItem CreateMoveItemGameObject()
    {
        InventoryItem inventoryItem = Instantiate(movePrefab).GetComponent<InventoryItem>();
        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        MoveItem moveItem = inventoryItem.gameObject.AddComponent<MoveItem>();
        moveItem.inventoryItem = inventoryItem;

        inventoryItem.Set(genericMovePrefab);

        return inventoryItem;
    }
    private void LeftMouseButtonPress()
    {
        Vector2Int tileGridPosition = GetTileGridPosition();

        if (selectedItem == null)
        {
            PickUpItem(tileGridPosition);
        }
        else
        {
            PlaceItem(tileGridPosition);
        }
    }
    private Vector2Int GetTileGridPosition()
    {
        Vector2 position = Input.mousePosition;

        if (selectedItem != null)
        {
            position.x -= (selectedItem.WIDTH - 1) * GridManager.tileSizeWidth / 2;
            position.y += (selectedItem.HEIGHT - 1) * GridManager.tileSizeHeight / 2;
        }

        return selectedItemGridManager.GetTileGridPosition(position);
    }
    private void PlaceItem(Vector2Int tileGridPosition)
    {
        bool complete = selectedItemGridManager.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y, ref overlapItem);
        if (complete)
        {
            selectedItem = null;
            if (overlapItem != null)
            {
                selectedItem = overlapItem;
                overlapItem = null;
                rectTransform = selectedItem.GetComponent<RectTransform>();
                rectTransform.SetAsLastSibling();
            }
        }
    }
    private void PickUpItem(Vector2Int tileGridPosition)
    {
        selectedItem = selectedItemGridManager.PickUpItem(tileGridPosition.x, tileGridPosition.y);
        if (selectedItem != null)
        {
            rectTransform = selectedItem.GetComponent<RectTransform>();
        }
    }
    private void ItemIconDrag()
    {
        if (selectedItem != null)
        {
            rectTransform.position = Input.mousePosition;
        }
    }
}
