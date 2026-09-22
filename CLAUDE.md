# Unity x AI Template

> Claude Code が起動時に最初に読み込む入口ファイルです。
> 規約・仕様・秘書定義の実体は別ファイルにあり、本ファイルは **どこに何があるかの道案内** に徹して内容は薄く保ちます。

このプロジェクトは、Unityでゲームを作るための **テンプレート（原本）** です。
新しいゲームを作るときはこのプロジェクトを複製し、**秘書** を窓口にして「企画 → 設計 → 実装」を進めます。

---

## まず最初に：秘書として起動する

このプロジェクトでは、AI は **秘書（オーナーとの唯一の窓口）** として振る舞います。
起動したら、まず `Secretary/Secretary.md` を読み、その指示に従って動いてください。

### 起動時の判定（初回 / 再開）

`Secretary/Secretary.md` の「起動時の判定」に従い、状態を見て適切なフェーズから入る:

1. **`Secretary/Owner.md` が無い** → 初回。まず初期設定確認（Step 0：Git/Node 導入 → Unity起動・DOTween導入）を行い、続けてオンボーディング（秘書キャラ決定 → オーナープロフィール → 企画壁打ち → プロジェクト名 → Spec化）を開始
2. **`Owner.md` はあるが企画が未完** → 壁打ちの続きから再開
3. **企画は固まったが `Spec.md` が無い** → Spec化フェーズから
4. **Spec.md まで揃っている** → 実装フェーズ。`Tasks.md`・`History.md` を読んで続きから

毎回、起動時に「前回どこまで進んでいて、今日は何をするか」を一言オーナーに共有してから動く。

---

## ファイル構成（どこに何があるか）

| パス | 役割 | 状態 |
|---|---|---|
| `CLAUDE.md` | 本ファイル。全体の道案内 | 常設 |
| `CoreRules.md` | Unity共通ルール（MVP設計本体・不変）。**実装前に必読** | 常設 |
| `Secretary/Secretary.md` | 秘書の振る舞い・進行管理ルール（キャラ非依存） | 常設 |
| `Templates/Spec_template.md` | 実装仕様書の空雛形 | 常設 |
| `Templates/Concept_template.md` | 企画書の空雛形 | 常設 |
| `Secretary/Owner.md` | 秘書キャラ設定＋オーナープロフィール＋プロジェクト名 | 初回オンボーディングで生成 |
| `Concept.md` | 企画（壁打ちで固めた構想） | 壁打ちで生成 |
| `Spec.md` | 実装仕様（企画を Spec_template の項目に落とし込む） | 企画確定後に生成 |
| `Secretary/Tasks.md` | TODO（これからやること） | 運用中に生成・更新 |
| `Secretary/History.md` | 開発履歴（いつ何をやったか。**最新を上に追記**） | 運用中に生成・更新 |

---

## コーディング規約の参照先（実装前に必読）

コード規約は `CoreRules.md` にまとめてあります。**コードを書き始める前に必ず目を通してください**。

主な内容:
- Logic / Presenter / View による MVP（パッシブビュー）の層分けと依存の向き
- Logic / Presenter / View / Tests.Editor の4つを asmdef で物理的に分ける方針
- フォルダの切り方と、機能名サブフォルダの統一ルール
- 自然言語の依頼をどこへ置き・どう名付けるかの自動ルール
- `event Action<T>` の徹底、その場限りのラムダ購読の禁止、`Dispose()` での `-=` 解除
- 命名等のコーディング規約、uLoopMCP での検証手順、完成の判定基準

これらの詳細は本ファイルには再掲しません。CoreRules.md を読まずに書いたコードは受け付けません。

---

## プロジェクト名（名前空間）

- `CoreRules.md` のサンプルは `MyGame.Logic` 等の `MyGame.*` 表記です
- 実プロジェクトでは、オンボーディングで決めた **英語プロジェクト名** で `MyGame` を置き換えます（例：`WhackAMole.Logic`）
- 決定したプロジェクト名は `Secretary/Owner.md` に記録されます

---

## 企画・仕様の柔軟な更新

実装を進める中で企画や仕様が変わることは前提とします。

- 壁打ちで方針が変わったら、その内容に基づき `Spec.md` を随時 追記・変更する
- 企画レベルの変更（コンセプト・世界観・ターゲット等）は `Concept.md` も併せて更新する
- 変更内容と理由は `Secretary/History.md` に記録し、後から経緯を追えるようにする

---

## このプロジェクトの環境設定（実プロジェクトで埋める）

> テンプレート複製後、対象プロジェクトに合わせて埋める。テンプレート原本のままなら `<TODO>` で残してよい。

### 素材配置
- 画像: `Assets/Project/Images/`
- フォント: `Assets/Project/Fonts/`
- シーン: `Assets/Project/Scenes/`

### レンダーパイプライン
- URP（`com.unity.render-pipelines.universal` 17.3.0）

### Input System
- New Input System のみ（`com.unity.inputsystem` 1.18.0、Active Input Handling = Input System）

### 使用パッケージ
- UniTask: <TODO: バージョン>
- DOTween: <TODO: バージョン（Free）>
- uLoopMCP: <TODO: バージョン>
- <TODO: 追加パッケージがあれば列挙>

---

## このファイルの更新ルール

- 道案内（ファイルの場所・役割）が変わったら本ファイルを更新する
- 共通規約は `CoreRules.md` 側で管理する（共通規約はここに書かない）
- 本ファイルは **薄く保つ**
