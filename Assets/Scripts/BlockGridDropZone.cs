﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockGridDropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rect;
    public GameManager gameManager = null;
    public BlockGridInfo blockGridInfo = null;
    public CanvasGroup canvas = null;
    public bool isInside = true;
    private Rect canvasRect;

    public static List<Transform> blockGrids = new List<Transform>();

    void Awake() {
        gameManager = GameUtility.getGameManager();
        blockGrids.Add( transform );
        rect = GetComponent<RectTransform>();
    }

    private void Update() {

        if ( transform.childCount == 0 && blockGridInfo.priority > 0 ) {
            if ( gameManager.isDraging && !transform.IsChildOf( gameManager.targetBlock ) ) {
                canvas.blocksRaycasts = true;
            }
            else {
                canvas.blocksRaycasts = false;
            }
        }
        if ( blockGridInfo.priority == 0 && transform.childCount > 0 ) {
            //isInside = false;

            //RectTransform[] check = { transform.GetChild(0).GetComponent<RectTransform>(), transform.GetChild( transform.childCount - 1 ).GetComponent<RectTransform>() };

            //// corners of item in world space
            //Vector3 localSpacePoint;

            //for ( int j = 0; j < 2; j++ ) {
            //    Vector3[] corners = new Vector3[4];
            //    check[j].GetWorldCorners( corners );
            //    for ( int i = 0; i < 4; i++ ) {
            //        // Backtransform to parent space
            //        localSpacePoint = gameManager.canvas.GetComponent<RectTransform>().InverseTransformPoint( corners[i] );

            //        // If parent (canvas) does not contain checked items any point
            //        if ( gameManager.gameContent.GetComponent<RectTransform>().rect.Contains( localSpacePoint ) ) {
            //            isInside = true;
            //        }
            //    }
            //}

        }
    }

    void Start() {

        if ( transform.parent.name.StartsWith( "Content" ) ) {
            blockGridInfo.priority = 0;
            PriorityGiving();
        }

        Resize();

    }

    public void InfoReset() {
        Transform target = transform;
        while ( !target.parent.name.StartsWith( "Content" ) ) {
            target = target.parent;
        }

        Debug.Log( target );

        target.GetComponent<BlockGridDropZone>().PriorityGiving();
        Resize();
    }

    public void PriorityGiving() {
        //transform.position = new Vector3( transform.position.x, transform.position.y, blockGridInfo.priority * 0.01f );
        for (int i = 0; i < transform.childCount; i++ ) {
            Transform child = transform.GetChild( i );

            //Debug.Log( child.GetComponent<BlockInfo>().blockType );

            switch ( child.GetComponent<BlockInfo>().blockType ) {
                case BlockType.IfBlock:
                case BlockType.ForBlock:
                case BlockType.RepeatBlock:
                    Debug.Log( child.GetComponent<BlockInfo>().refField.Count - 1 );
                    ToTheNextPriority( child.GetComponent<BlockInfo>().refField[child.GetComponent<BlockInfo>().refField.Count-1].GetComponent<ValueBlockSwap>().valueBlockGrid );
                    for (int j = 1; j < child.childCount; j+=2 ) {
                        ToTheNextPriority( child.GetChild( j ).GetChild( 1 ) );
                    }
                    break;
                case BlockType.SetBlock:
                case BlockType.AddBlock:
                    ToTheNextPriority( child.GetComponent<BlockInfo>().refField[0].GetComponent<ValueBlockSwap>().valueBlockGrid );
                    ToTheNextPriority( child.GetComponent<BlockInfo>().refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid );
                    break;
                case BlockType.DefineBlock:
                    ToTheNextPriority( child.GetComponent<BlockInfo>().refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid );
                    break;
                //case BlockType.MoveBlock:
                    //ToTheNextPriority( child.GetComponent<BlockInfo>().refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid );
                    //nextBlockGrid = child.GetComponent<BlockInfo>().refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid;
                    //nextBlockGridDropZone = nextBlockGrid.GetComponent<BlockGridDropZone>();
                    //nextBlockGridDropZone.blockGridInfo.priority = blockGridInfo.priority + 1;
                    //nextBlockGridDropZone.PriorityGiving();
                    //break;
                case BlockType.LogicBlock:
                    if ( child.GetComponent<BlockInfo>().refField[0].GetComponent<ValueBlockSwap>() != null ) {
                        ToTheNextPriority( child.GetComponent<BlockInfo>().refField[0].GetComponent<ValueBlockSwap>().valueBlockGrid );
                        ToTheNextPriority( child.GetComponent<BlockInfo>().refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid );
                    }
                    break;
            }
        }
    }

    private void ToTheNextPriority( Transform nextBlockGrid ) {
        BlockGridDropZone bgdz = nextBlockGrid.GetComponent<BlockGridDropZone>();
        bgdz.blockGridInfo.priority = blockGridInfo.priority + 1;
        bgdz.PriorityGiving();
    }

    private void OnDestroy() {
        blockGrids.Remove( transform );
    }

    public void Resize( float additional = 0f ) {
        rect.sizeDelta = Vector2.zero;
        float width = 0f;
        float height = 0f;

        bool force = false;
        ////////
        for ( int i = 0; i < transform.childCount; i++ ) {
            Transform child = transform.GetChild( i );
            switch ( child.GetComponent<BlockInfo>().blockType ) {
                case BlockType.IfBlock:
                    child.GetChild( 1 ).GetChild( 1 ).GetComponent<BlockGridDropZone>().Resize();
                    if ( child.childCount > 3 ) {
                        child.GetChild( 3 ).GetChild( 1 ).GetComponent<BlockGridDropZone>().Resize();
                    }
                    break;
                case BlockType.ForBlock:
                case BlockType.RepeatBlock:
                    child.GetChild( 1 ).GetChild( 1 ).GetComponent<BlockGridDropZone>().Resize();
                    break;
            }
        }
        ///////

        switch ( blockGridInfo.blockGridType ) {
            case BlockGridType.Block:
                for ( int i = 0; i < transform.childCount; i++ ) {
                    Transform child = transform.GetChild( i );
                    RectTransform cRect;
                    if ( ( cRect = child.GetComponent<RectTransform>() ) != null ) {
                        width = Mathf.Max( width, cRect.sizeDelta.x );
                        height += cRect.sizeDelta.y;
                    }
                }

                height -= Mathf.Max( 0f, transform.childCount - 1 ) * GameUtility.CONNECTOR_HEIGHT;

                bool extend = false;
                if ( transform.childCount > 0 ) {
                    if ( transform.parent.name.StartsWith( "Content" ) ) {
                        if ( gameManager.isDraging ) {
                            extend = true;
                        }
                    }
                    else if ( gameManager.blockGridsUnderPointer.Contains( transform ) ) {
                        if ( transform.childCount > 0 && gameManager.isDraging ) {
                            extend = false;
                        }
                    }

                    if ( transform.GetChild( 0 ).GetComponent<BlockInfo>().connectRule[1] == false ) {
                        extend = false;
                    }
                }
                else {
                    extend = true;
                }

                if ( extend ) {
                    if ( width == 0f ) width = GameUtility.BLOCK_WIDTH;
                    if ( height == 0f ) height += GameUtility.CONNECTOR_HEIGHT;
                    height += GameUtility.BLOCK_HEIGHT - GameUtility.CONNECTOR_HEIGHT;
                }

                if ( transform.parent.name.StartsWith( "Beam" ) ) {
                    transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2( transform.parent.GetComponent<RectTransform>().sizeDelta.x, height );
                    transform.parent.GetChild( 0 ).GetComponent<RectTransform>().sizeDelta = new Vector2( transform.parent.GetChild( 0 ).GetComponent<RectTransform>().sizeDelta.x, height );
                    Transform block = transform.parent.parent;
                    float blockHeight = 0f;
                    for ( int i = 0; i < block.childCount; i++ ) {
                        blockHeight += block.GetChild( i ).GetComponent<RectTransform>().sizeDelta.y - GameUtility.CONNECTOR_HEIGHT;
                    }
                    block.GetComponent<RectTransform>().sizeDelta = new Vector2( block.GetComponent<RectTransform>().sizeDelta.x, blockHeight + GameUtility.CONNECTOR_HEIGHT );
                }

                break;

            case BlockGridType.Value:
                if ( transform.childCount > 0 ) {
                    //SizeFitter.CheckForChanges( transform );
                    height = 0;
                    width = 0;
                    force = true;
                }
                else {
                    if ( transform.parent.name.StartsWith( "BlockSpawner" ) ) {
                        height = GameUtility.VALUE_HEIGHT;
                        width = 0;
                    }
                    else {
                        //height = GameUtility.VALUE_HEIGHT;
                        //width = GameUtility.VALUE_WIDTH;
                    }
                }
                break;
        }
        if ( !force ) {
            rect.sizeDelta = new Vector2( Mathf.Max( width, rect.sizeDelta.x ), Mathf.Max( height + additional, rect.sizeDelta.y ) );
        }
        else {
            rect.sizeDelta = new Vector2( width, height );
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {

        if ( GetComponent<CanvasGroup>().blocksRaycasts == true ) {
        
        }
        else {
            Debug.Log( "what" );
            return;
        }

        gameManager.blockGridsUnderPointer.Add( transform );
    }
    public void OnPointerExit(PointerEventData eventData) {

        //Debug.Log( "Leave to :" + transform );

        gameManager.blockGridsUnderPointer.Remove( transform );
    }

    public void OnDrop(PointerEventData eventData)
    {
        //GameObject holding = eventData.pointerDrag;
        //Debug.Log(holding.name + " is being drop on " + name);

        //Transform oldParent = holding.transform.parent;

        //int i = 0;
        //foreach (Transform child in holding.GetComponent<BlockPhysic>().getChildList())
        //{
        //    Debug.Log(child);
        //    child.SetParent(transform.parent);
        //    if (eventData.position.y < transform.position.y)
        //    {
        //        Debug.Log("Down");
        //        child.SetSiblingIndex(transform.GetSiblingIndex() + i++ + 1);
        //    }
        //    else
        //    {
        //        Debug.Log("Up");
        //        child.SetSiblingIndex(transform.GetSiblingIndex());
        //    }
        //}

        //if (oldParent.name.StartsWith("BlockGrid") && oldParent.childCount == 0)
        //{
        //    Destroy(oldParent.gameObject);
        //}

        //rect.sizeDelta = new Vector2( 300f, (transform.childCount+1) * 50f );

        //string codeString = "";

        //for (int j = 0; j < transform.parent.childCount; j++)
        //{
        //    codeString += transform.parent.GetChild(j).GetComponent<BlockInfo>().getBlockCodeString() + "\n";
        //}
        //Debug.Log(codeString);
    }
}
