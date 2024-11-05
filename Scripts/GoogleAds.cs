using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GoogleMobileAds.Api;

public class AdMobBanner : MonoBehaviour
{
    //やること
    //1.バナー広告IDを入力
    //2.バナーの表示位置　(現状表示位置は下になっています。)
    //3.バナー表示のタイミング (現状 起動直後になっています。)

    private BannerView bannerView;//BannerView型の変数bannerViewを宣言　この中にバナー広告の情報が入る


    //シーン読み込み時からバナーを表示する
    //最初からバナーを表示したくない場合はこの関数を消してください。
void Start()
{
    MobileAds.RaiseAdEventsOnUnityMainThread = true;

    MobileAds.Initialize(initStatus => {
        Debug.Log("Google Mobile Ads SDK Initialized");
        RequestBanner();
    });

}





    //ボタン等に割り付けて使用
    //バナーを表示する関数
    public void BannerStart()
    {
        RequestBanner();//アダプティブバナーを表示する関数 呼び出し       
    }

    //ボタン等に割り付けて使用
    //バナーを削除する関数
    public void BannerDestroy()
    {
        bannerView.Destroy();//バナー削除
    }

    //アダプティブバナーを表示する関数
    private void RequestBanner()
    {
        //AndroidとiOSで広告IDが違うのでプラットフォームで処理を分けます。
        // 参考
        //【Unity】AndroidとiOSで処理を分ける方法
        // https://marumaro7.hatenablog.com/entry/platformsyoriwakeru

#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-1706780047482878/5547047240";//ここにAndroidのバナーIDを入力
        
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/2934735716";//ここにiOSのバナーIDを入力

#else
        string adUnitId = "unexpected_platform";
#endif

        // 新しい広告を表示する前にバナーを削除
        if (bannerView != null)//もし変数bannerViewの中にバナーの情報が入っていたら
        {
            bannerView.Destroy();//バナー削除
        }

        //現在の画面の向き横幅を取得しバナーサイズを決定
        AdSize adaptiveSize =
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);


        //バナーを生成 new BannerView(バナーID,バナーサイズ,バナー表示位置)
        bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Bottom);//バナー表示位置は
                                                                               //画面上に表示する場合：AdPosition.Top
                                                                               //画面下に表示する場合：AdPosition.Bottom


        //BannerView型の変数 bannerViewの各種状態 に関数を登録
        bannerView.OnBannerAdLoaded += OnBannerAdLoaded;//bannerViewの状態が バナー表示完了 となった時に起動する関数(関数名OnBannerAdLoaded)を登録
        bannerView.OnBannerAdLoadFailed += OnBannerAdLoadFailed;//bannerViewの状態が バナー読み込み失敗 となった時に起動する関数(関数名OnBannerAdLoadFailed)を登録


        //リクエストを生成
        AdRequest adRequest = new AdRequest();

        //広告のキーワードを追加
        //===================================================================
        // アプリに関連するキーワードを文字列で設定するとアプリと広告の関連性が高まります。
        // 結果、収益が上がる可能性があります。
        // 任意設定のため不要であれば消していただいて問題はありません。

        // Application.systemLanguageでOSの言語判別　
        // 返り値はSystemLanguage.言語
        // 端末の言語が日本語の時
        if (Application.systemLanguage == SystemLanguage.Japanese)
        {
            adRequest.Keywords.Add("ゲーム");
            adRequest.Keywords.Add("モバイルゲーム");
        }

        //端末の言語が日本語以外の時
        else
        {
            adRequest.Keywords.Add("game");
            adRequest.Keywords.Add("mobile games");
        }
        //==================================================================


        //広告表示
        bannerView.LoadAd(adRequest);
    }


    #region Banner callback handlers

    //バナー表示完了 となった時に起動する関数
    private void OnBannerAdLoaded()
    {
            Debug.Log("ロードされました - 表示します");
            bannerView.Show();
    }

    //バナー読み込み失敗 となった時に起動する関数
    private void OnBannerAdLoadFailed(LoadAdError error)
    {
        Debug.Log("バナー読み込み失敗" + error);//error:エラー内容 
    }

    #endregion
}
