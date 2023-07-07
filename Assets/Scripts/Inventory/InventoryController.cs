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
    //Constant variables
    [SerializeField] GridManager dropGridManager;
    [SerializeField] InventoryStatUserInterface inventoryStatUI;
    [SerializeField] PlayerInformation playerInformation;
    private GearData gearData;

    InventoryItem selectedItem;
    InventoryItem overlapItem;
    RectTransform rectTransform;

    [SerializeField] List<InventoryItemData> items;
    [SerializeField] InventoryItemData genericMoveInventoryItemData;

    [SerializeField] GameObject movePrefab;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;

    InventoryHighlight inventoryHighlight;

    Vector2Int oldPosition;
    InventoryItem itemToHighlight;

    #region test
    //TESTING VARIABLES
/*    public InventoryItemData testInventoryData;
    public MovesData testMove;*/
    #endregion

    private void Awake()
    {
        inventoryHighlight = GetComponent<InventoryHighlight>();
        gearData = gameObject.AddComponent<GearData>();
    }

    // Update is called once per frame
    void Update()
    {
        ItemIconDrag();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            //InsertRandomItemInDropGrid();
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
    public void InsertRandomItemInDropGrid()
    {
        if (dropGridManager == null || selectedItem != null) { return; }

        InventoryItem itemToInsert = CreateRandomItem();
        if (!InsertItemToDropGrid(itemToInsert))
        {
            Destroy(itemToInsert.gameObject);
        }
    }
    public void InsertItemInDropGrid(InventoryItemData item, MovesData moveData)
    {
        if (dropGridManager == null || selectedItem != null) { return; }
        if (item == null && moveData == null) { Debug.LogError("Item and move data missing to insert item");  return; }

        if (moveData == null)
        {
            Debug.Log("Insert Item");
            InventoryItem itemToInsert = CreateSpecificItem(item);
            if (!InsertItemToDropGrid(itemToInsert))
            {
                Debug.Log($"{itemToInsert.gameObject.name} was destoryed");
                Destroy(itemToInsert.gameObject);
            }
        }
        else
        {
            Debug.Log("Insert Move Item");
            InventoryItem itemToInsert = CreateMoveItemGameObject(moveData);
            if (!InsertItemToDropGrid(itemToInsert))
            {
                Debug.Log($"{itemToInsert.gameObject.name} was destoryed");
                Destroy(itemToInsert.gameObject);
            }
        }
    }
    private bool InsertItemToDropGrid(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = dropGridManager.FindSpaceForObject(itemToInsert);
        if (posOnGrid == null) { return false;  }
        dropGridManager.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
        return true;
    }
    public void InsertPlayerInitalMoves(UnitInstance playerUnitInstance, List<GameObject> moveGameObjectList)
    {
        for (int moveNum = 0; moveNum < moveGameObjectList.Count; moveNum++)
        {
            if (playerUnitInstance.moveList[moveNum] == null)
            {
                return;
            }
            //Create item gameobject and assign move information on the gameobject
            InventoryItem moveItem = CreateMoveItemGameObject(playerUnitInstance.moveList[moveNum]);
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
    private InventoryItem CreateRandomItem()
    {
        //Create a phyical image for the inventory item that will be stored in the grid
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);

        int selectedItemId = UnityEngine.Random.Range(0, items.Count);
        inventoryItem.Set(items[selectedItemId]);
        if (inventoryItem.data.itemClass == ItemClassification.Head || inventoryItem.data.itemClass == ItemClassification.Chest || inventoryItem.data.itemClass == ItemClassification.Leg)
        {
            AddGearStat(inventoryItem);
        }
        else if (inventoryItem.data.itemClass == ItemClassification.Weapon)
        {

        }
        else if (inventoryItem.data.itemClass == ItemClassification.Consumable)
        {

        }
        return inventoryItem;
    }
    private InventoryItem CreateSpecificItem(InventoryItemData inventoryItemData)
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        inventoryItem.Set(inventoryItemData);
        return inventoryItem;
    }
    private InventoryItem CreateMoveItemGameObject(MovesData moveItemData)
    {
        InventoryItem inventoryItem = Instantiate(movePrefab).GetComponent<InventoryItem>();
        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        MoveItem moveItem = inventoryItem.gameObject.AddComponent<MoveItem>();
        moveItem.inventoryItem = inventoryItem;
        inventoryItem.GetComponent<MoveItem>().moveData = moveItemData;

        inventoryItem.Set(genericMoveInventoryItemData);
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
            playerInformation.SetPlayerGearStatus();
            inventoryStatUI.UpdatePlayerStatUserInterface();
            inventoryStatUI.SetAboutItemUserInterface(false);
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
            inventoryStatUI.UpdateItemAboutUserInterface(selectedItem);
            inventoryStatUI.SetAboutItemUserInterface(true);
        }
    }
    private void ItemIconDrag()
    {
        if (selectedItem != null)
        {
            rectTransform.position = Input.mousePosition;
        }
    }
    private void AddGearStat(InventoryItem inventoryItem)
    {
        if (inventoryItem.data.itemClass == ItemClassification.Head)
        {
            GearStats gs = new(gearData.GetHeadMainStat(), gearData.GetRandomPositiveStatNum());
            inventoryItem.gearStatList.Add(gs);
        }
        else if (inventoryItem.data.itemClass == ItemClassification.Chest)
        {
            GearStats gs = new(gearData.GetChestMainStat(), gearData.GetRandomPositiveStatNum());
            inventoryItem.gearStatList.Add(gs);
        }
        else if (inventoryItem.data.itemClass == ItemClassification.Leg)
        {
            GearStats gs = new(gearData.GetLegMainStat(), gearData.GetRandomPositiveStatNum());
            inventoryItem.gearStatList.Add(gs);
        }
        GearStats gs1 = new(gearData.GetRandomStat(), gearData.GetRandomPositiveStatNum());
        inventoryItem.gearStatList.Add(gs1);
        GearStats gs2 = new(gearData.GetRandomStat(), gearData.GetRandomPositiveStatNum());
        inventoryItem.gearStatList.Add(gs2);
    }
}
