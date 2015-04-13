### Web アプリケーションの設定
* Web.config で既定のドキュメントに ClickOnceWpf2.application を追加する

### 証明書
* abc-root.cer を証明書ストアのルート (Root) にインストールする
  * 証明書チェーンが検証されないと発行に失敗する
* abc-test.pfx を証明書ストアの個人 (My) にインストールする

### プロジェクトの設定

#### 署名
[ストアから選択] → [Abc Test]

#### 発行
* 発行フォルダーの場所: ..\ClickOnceWeb2\
* 発行者名: Abc Publisher
* スイート名: Abc WPF
* 製品名: ClickOnce WPF 2

### 発行
* ソリューション構成を [Release] に切り替える
* [今すぐ発行]

### 実行結果
* ルート証明書を Root に、発行元証明書を TrustedPublisher に登録しても、赤色の警告

### 自動更新
アプリケーションの自動更新を有効にする場合、[インストール フォルダーの URL] を指定する。
ただし、localhost の Web サーバーでは失敗する。
