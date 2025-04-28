#nullable enable
using GameCanvas;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ゲームクラス。
/// 学生が編集すべきソースコードです。
/// </summary>
public sealed class Game : GameBase
{
    // 解像度
    const int SCREEN_WIDTH = 640;
    const int SCREEN_HEIGHT = 480;

    // ボールに関する変数
    int ball_x;
    int ball_y;
    int ball_speed_x;
    int ball_speed_y;

    // 当たり判定時ボールを矩形に近似するための定数
    const int BALL_RAD = 20;

    // 自機(バー)に関する処理
    int player_x;
    int player_y;
    int player_w;
    int player_h;

    // ブロックに関する変数
    const int BLOCK_NUM = 50;
    int currentBlockCount;
    int[] block_x = new int[BLOCK_NUM];
    int[] block_y = new int [BLOCK_NUM];
    bool[] isBlockAlive = new bool [BLOCK_NUM];
    int block_w = 64;
    int block_h = 20;
    int time;

    /// <summary>
    /// 初期化処理
    /// </summary>
    public override void InitGame()
    {
        gc.SetResolution(SCREEN_WIDTH, SCREEN_HEIGHT);

        player_x = 270;
        player_y = 460;
        player_w = 100;
        player_h = 20;

        ball_x = player_x - 100;
        ball_y = player_y - 100;
        ball_speed_x = 3;
        ball_speed_y = 3;

        for(int i=0; i<BLOCK_NUM; i++){
            block_x[i] = (i % 10) * block_w;
            block_y[i] = (i / 10) * block_h;
            isBlockAlive[i] = true;
        }
        currentBlockCount = BLOCK_NUM;
        time = 0;
    }

    /// <summary>
    /// 動きなどの更新処理
    /// </summary>
    public override void UpdateGame()
    {
        ball_x += ball_speed_x;
        ball_y += ball_speed_y;
        // 壁に当たったら反射
        if(ball_x < 0){
            ball_x = 0;
            ball_speed_x *= -1;
        }
        if(ball_y < 0){
            ball_y = 0;
            ball_speed_y *= -1;
        }
        if(ball_x > SCREEN_WIDTH - BALL_RAD){
            ball_x = SCREEN_WIDTH - BALL_RAD;
            ball_speed_x *= -1;
        }

        // マウス操作でバーを動かす処理
        if(gc.GetPointerFrameCount(0) > 0){
            player_x = (int)gc.GetPointerX(0) - player_w/2;
            player_y = (int)gc.GetPointerY(0) - player_h/2;
        }

        // 壁への当たり判定の命令
        if(gc.CheckHitRect(ball_x, ball_y, BALL_RAD, BALL_RAD, player_x, player_y, player_w, player_h)){
            if(ball_speed_y>=0){
                ball_speed_y *= -1;
            }
        }

        // ブロックへの当たり判定の命令
        for(int i=0; i<BLOCK_NUM; i++){
            if(gc.CheckHitRect(ball_x, ball_y, BALL_RAD, BALL_RAD, block_x[i], block_y[i], block_w-4, block_h-4)){
                if(isBlockAlive[i]){
                    ball_speed_y *= -1;
                    isBlockAlive[i] = false;
                    currentBlockCount--;
                }
            }
        }

        // 全消しまでのフレームカウント
        if(currentBlockCount>0)
            time++;

        // 任意のキー押下でリセット処理
        if(gc.IsAnyKeyDown)
            InitGame();
    }

    /// <summary>
    /// 描画の処理
    /// </summary>
    public override void DrawGame()
    {
        // 画面を白で塗りつぶします
        gc.ClearScreen();

        // 青空の画像を描画します
        gc.DrawImage(GcImage.BlueSky, 0, 0);

        gc.DrawImage(GcImage.BallYellow, ball_x, ball_y);

        gc.SetColor(0, 0, 255);
        gc.FillRect(player_x, player_y, player_w, player_h);

        gc.DrawString("TIME: " + time, SCREEN_WIDTH/2, SCREEN_HEIGHT/4*3);
        // クリア時の表示
        if(currentBlockCount==0)
            gc.DrawString("CLEARED", SCREEN_WIDTH/2, SCREEN_HEIGHT/2);

        for(int i=0; i<BLOCK_NUM; i++){
            if(isBlockAlive[i]){
                if(i%10==0 || i%10==3 || i%10==6 || i%10==9){
                    gc.SetColor(255,0,0);
                    gc.FillRect(block_x[i], block_y[i], block_w-4, block_h-4);
                }
                if(i%10==1 || i%10==4 || i%10==7){
                    gc.SetColor(0,255,0);
                    gc.FillRect(block_x[i], block_y[i], block_w-4, block_h-4);
                }
                if(i%10==2 || i%10==5 || i%10==8){
                    gc.SetColor(0,0,255);
                    gc.FillRect(block_x[i], block_y[i], block_w-4, block_h-4);
                }
            }
        }
    }
}
