### Web アプリケーションの設定
* Web.config で既定のドキュメントに ClickOnceWpf2.application を追加する

### 証明書
* abc-root.cer を証明書ストアのルート (Root) にインストールする
* abc-test.pfx を証明書ストアの個人 (My) にインストールする

登録しないと発行に失敗する。

### プロジェクトの設定

#### 署名
[ストアから選択] → [Abc Test]

#### 発行
* 発行フォルダーの場所: ..\ClickOnceWeb2\
* インストール フォルダーの URL: http://localhost:45869/
* 発行者名: Abc Publisher
* スイート名: Abc WPF
* 製品名: ClickOnce WPF 2

### 発行
* ソリューション構成を [Release] に切り替える
* [今すぐ発行]

### 実行結果
* ルート証明書を Root に、発行元証明書を TrustedPublisher に登録しても、赤色の警告
