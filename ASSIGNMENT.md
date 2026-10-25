# تكليف عملي ومشروع تدريبي: بناء Web API لمتجر إلكتروني مع كافة علاقات Entity Framework Core
### Practical Assignment: E-Commerce Product Management Web API with Full EF Core Relationships

---

## 1. نبذة عن التكليف والهدف التعليمي (Assignment Overview & Objectives)

**الهدف من التكليف:**
يهدف هذا التكليف إلى تقييم وتدريب الطالب على بناء واجهة برمجية متكاملة (RESTful Web API) باستخدام **ASP.NET Core** و **Entity Framework Core (Code-First)**، وتطبيق المفاهيم المتقدمة في هندسة قواعد البيانات العلائقية:
1. بناء وتمثيل **جميع أنواع العلاقات في قواعد البيانات**:
   - علاقة واحد لواحد (**1:1 - One-to-One**)
   - علاقة واحد لمتعدد (**1:N - One-to-Many**) من جهة الأب والابن
   - علاقة متعدد لمتعدد (**N:M - Many-to-Many**) عبر جدول وسيط صريح (Join Entity)
2. استخدام كائنات نقل البيانات (**DTOs**) للفصل التام بين قاعدة البيانات والـ API Contract.
3. تطبيق مبادئ الـ **Eager Loading** باستخدام `.Include()` و `.ThenInclude()`.
4. ضبط قواعد سلامة البيانات والمسح التعاقبي (**Cascade Delete / Restrict**).
5. دعم العمليات الكاملة (**CRUD**) مع التعديل الجزئي (**Partial Update / HTTP Patch**).

---

## 2. سيناريو النظام (Business Scenario)

أنت مهندس برمجيات مكلف بتطوير الـ Backend لنظام كتالوج منتجات متجر إلكتروني متقدم (**E-Commerce Product Catalog**). النظام يتعامل مع عدة أنواع من البيانات المترابطة وفق السيناريو التالي:

### أ) الأقسام والمنتجات (Categories & Products):
* يتكون المتجر من مجموعة من الأقسام الأساسية مثل: *"الأجهزة الإلكترونية"*, *"الكتب والبرمجيات"*.
* كل منتج يتبع بالضرورة لقسم واحد فقط، في حين يمكن للقسم الواحد أن يحتوي على عدد غير محدود من المنتجات.
* **قاعدة عمل:** لا يُسمح بحذف أي قسم طالما توجد منتجات مرتبطة به حفاظاً على سلامة البيانات.

### ب) تمايز نوع المنتجات (Product Profiles):
المنتجات في المتجر تنقسم لنوعين رئيسيين عبر حقل نوع المنتج (`ProductType`):
1. **منتج مادي (Physical Product):** مثل الحواسيب والأجهزة. يحتاج النظام لحفظ تفاصيل شحنه في جدول منفصل يحتوي على (الوزن بالكيلوجرام، أبعاد الشحنة، وموقع الرف في المستودع).
2. **منتج رقمي (Digital Product):** مثل الكتب الإلكترونية والتراخيص. يحتاج النظام لحفظ تفاصيل تحميله في جدول منفصل يحتوي على (رابط التحميل المباشر، حجم الملف بالميغابايت، وصيغة الملف كالـ PDF أو ZIP).
* **قاعدة عمل:** كل منتج يرتبط بملف تفاصيل واحد فقط من نوعه، وتكون العلاقة فريدة تماماً عبر فهرس فريد (`Unique Index`).

### ج) تقييمات ومراجعات العملاء (Product Reviews):
* يستطيع عملاء المتجر ترك مراجعات وتقييمات للمنتجات المشتراة تشمل (اسم العميل، التقييم من 1 إلى 5 نجوم، وتعليق المراجعة، وتاريخ الإنشاء).
* المنتج الواحد يمتلك قائمة مراجعات متعددة.
* **قاعدة عمل:** عند حذف منتج، يجب أن تُحذف تلقائياً كافة المراجعات المرتبطة به (**Cascade Delete**).

### د) الوسوم والتصنيفات الترويجية (Tags & ProductTags):
* يمتلك المتجر وسوماً تسويقية مثل: *"الأكثر مبيعاً"*, *"عرض خاص"*, *"جديد"*.
* الوسم الواحد يمكن وضعه على عدة منتجات، والمنتج الواحد يمكن أن يحمل أكثر من وسم.
* يتم الربط بينهما عبر جدول وسيط صريح يسجل تاريخ إسناد الوسم للمنتج، ويمنع تكرار نفس الوسم للمنتج الواحد.
* **قاعدة عمل:** عند حذف المنتج أو حذف الوسم، يتم حذف سجل الربط تلقائياً (**Cascade Delete**).

---

## 3. مواصفات الكيانات والعلاقات (Database Schema & Relationships)

```mermaid
erDiagram
    CATEGORY ||--o{ PRODUCT : "1:N (One Category has Many Products) - Restrict"
    PRODUCT ||--o{ PRODUCT_REVIEW : "1:N (One Product has Many Reviews) - Cascade"
    PRODUCT ||--o| PHYSICAL_PRODUCT_PROFILE : "1:1 (Unique Physical Profile)"
    PRODUCT ||--o| DIGITAL_PRODUCT_PROFILE : "1:1 (Unique Digital Profile)"
    PRODUCT ||--o{ PRODUCT_TAG : "N:M (Join Entity) - Cascade"
    TAG ||--o{ PRODUCT_TAG : "N:M (Join Entity) - Cascade"

    CATEGORY {
        int Id PK
        string Name
        string Description
    }

    PRODUCT {
        int Id PK
        string Name
        string Sku UK
        decimal Price
        DateTime CreatedAt
        int Type
        int CategoryId FK
        int PhysicalProfileId FK-UK
        int DigitalProfileId FK-UK
    }

    PHYSICAL_PRODUCT_PROFILE {
        int Id PK
        double WeightKg
        string Dimensions
        string WarehouseLocation
    }

    DIGITAL_PRODUCT_PROFILE {
        int Id PK
        string DownloadUrl
        double FileSizeMB
        string FileFormat
    }

    PRODUCT_REVIEW {
        int Id PK
        string ReviewerName
        int Rating
        string Comment
        DateTime CreatedAt
        int ProductId FK
    }

    TAG {
        int Id PK
        string Name
    }

    PRODUCT_TAG {
        int Id PK
        int ProductId FK
        int TagId FK
        DateTime AssignedAt
    }
```

---

## 4. الهيكلية المعمارية المطلوبة (Project Architecture)

يجب تنظيم الكود وفق البنية التالية لضمان نظافة الكود وفصل المسؤوليات (Separation of Concerns):

```
EcommerceAPI_AI/
├── Context/
│   └── AppDbContext.cs               # تكوين قاعدة البيانات وضبط Fluent API والـ Cascades
├── Entities/
│   ├── Category.cs                  # كيان القسم (1:N)
│   ├── Product.cs                   # الكيان المركزي للـ Product وعلاقاته
│   ├── PhysicalProductProfile.cs    # كيان تفاصيل المنتج المادي (1:1)
│   ├── DigitalProductProfile.cs     # كيان تفاصيل المنتج الرقمي (1:1)
│   ├── ProductReview.cs             # كيان المراجعات (1:N)
│   ├── Tag.cs                       # كيان الوسم (N:M)
│   └── ProductTag.cs                # كيان جدول الوسيط (N:M)
├── Dtos/
│   ├── CategoryDtos.cs              # نماذج إدخال واستخراج الأقسام
│   ├── CreateProductInput.cs        # نموذج إضافة منتج مع كافة تفاصيله
│   ├── UpdateProductInput.cs        # نموذج تعديل بيانات المنتج والوسوم
│   ├── UpdateProductPriceInput.cs   # نموذج التعديل الجزئي لسعر المنتج
│   ├── ProductOutput.cs             # نموذج إرجاع المنتج الشامل لكافة العلاقات
│   ├── ProductReviewDtos.cs         # نماذج إضافة واستخراج المراجعات
│   └── TagDtos.cs                   # نماذج إضافة واستخراج الوسوم
└── Controllers/
    ├── CategoriesController.cs      # إدارة الأقسام
    ├── TagsController.cs            # إدارة الوسوم
    └── ProductsController.cs        # إدارة المنتجات وعلاقاتها
```

---

## 5. مواصفات الـ API Endpoints المطلوبة

### أ) إدارة المنتجات (`api/Products`)

| HTTP Method | الـ Endpoint | الوظيفة والمتطلبات | العلاقات المعنية |
|---|---|---|---|
| `POST` | `/api/Products/CreateProduct` | إنشاء منتج جديد مع بروفايله (مادي أو رقمي)، وربطه بالقسم، وإسناد الوسوم الأولية. | 1:1, 1:N, N:M |
| `PUT` | `/api/Products/UpdateProduct/{id}` | تعديل بيانات المنتج، بروفايله، قسمه، ومزامنة وسومه. | 1:1, 1:N, N:M |
| `PATCH` | `/api/Products/UpdateProductPrice/{id}` | تعديل جزئي على حقل السعر فقط وحفظ التغيير. | لا ينطبق |
| `GET` | `/api/Products/GetProduct/{id}` | جلب منتج محدد مع كافة علاقاته عبر `.Include()` و `.ThenInclude()`. | 1:1, 1:N, N:M |
| `GET` | `/api/Products/GetProducts` | جلب قائمة كاملة بالمنتجات مع تضمين كافة الكائنات المرتبطة. | 1:1, 1:N, N:M |
| `DELETE` | `/api/Products/DeleteProduct/{id}` | حذف المنتج مع حذف البروفايل الخاص به، والتأكد من حذف المراجعات وسجلات الوسوم تلقائياً. | 1:1, 1:N, N:M |
| `POST` | `/api/Products/AddReview/{productId}` | إضافة مراجعة جديدة للمنتج ديناميكياً. | 1:N |
| `POST` | `/api/Products/AssignTags/{productId}` | إسناد وسوم جديدة للمنتج ومنع التكرار. | N:M |

### ب) إدارة الأقسام (`api/Categories`)

| HTTP Method | الـ Endpoint | الوظيفة |
|---|---|---|
| `POST` | `/api/Categories/CreateCategory` | إضافة قسم جديد. |
| `GET` | `/api/Categories/GetCategories` | جلب الأقسام مع عدد المنتجات التابعة لكل قسم (`ProductCount`). |
| `GET` | `/api/Categories/GetCategory/{id}` | جلب بيانات قسم محدد. |
| `DELETE` | `/api/Categories/DeleteCategory/{id}` | حذف القسم (يجب أن يفشل ويرجع خطأ إذا كان يحتوي على منتجات). |

### ج) إدارة الوسوم (`api/Tags`)

| HTTP Method | الـ Endpoint | الوظيفة |
|---|---|---|
| `POST` | `/api/Tags/CreateTag` | إضافة وسم جديد. |
| `GET` | `/api/Tags/GetTags` | جلب كافة الوسوم المتاحة. |
| `DELETE` | `/api/Tags/DeleteTag/{id}` | حذف وسم (تُحذف روابطه مع المنتجات تلقائياً). |

---

## 6. نماذج طلبات واستجابات الـ API (Request & Response Samples)

### 1) إنشاء منتج مادي (Physical Product with 1:1, 1:N, N:M):
* **Endpoint:** `POST /api/Products/CreateProduct`
* **Request Body:**
```json
{
  "name": "Lenovo Legion Pro 7",
  "sku": "NB-LEGION-001",
  "price": 2200.00,
  "type": 1,
  "categoryId": 1,
  "physical": {
    "weightKg": 2.8,
    "dimensions": "36x26x2.5 cm",
    "warehouseLocation": "Row B, Shelf 4"
  },
  "tagIds": [1, 2]
}
```
* **Expected Response (`200 OK`):**
```json
{
  "id": 1,
  "name": "Lenovo Legion Pro 7",
  "sku": "NB-LEGION-001",
  "price": 2200.00,
  "createdAt": "2026-10-25T14:30:00Z",
  "type": 1,
  "category": {
    "id": 1,
    "name": "Laptops & Computers"
  },
  "physical": {
    "id": 1,
    "weightKg": 2.8,
    "dimensions": "36x26x2.5 cm",
    "warehouseLocation": "Row B, Shelf 4"
  },
  "digital": null,
  "reviews": [],
  "tags": [
    { "id": 1, "name": "Featured" },
    { "id": 2, "name": "BestSeller" }
  ]
}
```

---

### 2) إنشاء منتج رقمي (Digital Product with 1:1, 1:N):
* **Endpoint:** `POST /api/Products/CreateProduct`
* **Request Body:**
```json
{
  "name": "ASP.NET Core Masterclass",
  "sku": "COURSE-ASPNET-10",
  "price": 99.99,
  "type": 2,
  "categoryId": 2,
  "digital": {
    "downloadUrl": "https://cdn.example.com/courses/aspnet10.zip",
    "fileSizeMB": 1250.0,
    "fileFormat": "ZIP"
  },
  "tagIds": [1]
}
```

---

### 3) إضافة مراجعة للمنتج (1:N):
* **Endpoint:** `POST /api/Products/AddReview/1`
* **Request Body:**
```json
{
  "reviewerName": "Omar Developer",
  "rating": 5,
  "comment": "Exceptional build quality and great performance!"
}
```

---

### 4) تحديث سعر المنتج (HTTP Patch):
* **Endpoint:** `PATCH /api/Products/UpdateProductPrice/1`
* **Request Body:**
```json
{
  "price": 1999.00
}
```

---

## 7. حالات الاختبار والمعالجة المنطقية (Edge Cases & Business Rules)

على الطالب التأكد من استيفاء واختبار الحالات التالية:
1. **التحقق من صحة القسم (Foreign Key Validation):** إذا قام المستخدم بإرسال `categoryId` غير موجود، يجب إرجاع `400 Bad Request`.
2. **التحقق من ملف التفاصيل (Profile Validation):** إذا كان نوع المنتج `Physical` وتم إرسال الكائن `physical` كـ `null`، يجب رفض العملية برسالة واضحة.
3. **فرادة الرمز التعريفي (SKU Uniqueness):** حقل `Sku` يجب أن يكون فريداً عبر فهرس `[Index(nameof(Sku), IsUnique = true)]`.
4. **حظر حذف القسم المرتبط بمنتجات (Restrict Delete):** محاولة حذف قسم لديه منتجات يجب أن تُرفض مع إشعار المستخدم بوجود منتجات تابعة.
5. **حذف المنتج والمراجعات (Cascade Delete):** عند حذف المنتج، يجب التأكد من حذف المراجعات وسجلات الربط في جدول `ProductTags` وسجل الـ Profile المرتبط تلقائياً دون ترك بيانات يتيمة (Orphaned Records).
6. **منع تكرار الوسوم في جدول الوسيط:** استخدام فهرس مركب فريد لمنع إسناد نفس الوسم أكثر من مرة لنفس المنتج:
   ```csharp
   [Index(nameof(ProductId), nameof(TagId), IsUnique = true)]
   ```

---

## 8. سلّم التقييم وتوزيع الدرجات (Grading Rubric - 100 Marks)

| البند التقييمي | التفاصيل والمعايير | الدرجة |
|---|---|---|
| **1) تصميم الكيانات والفهارس والعلاقات** | - التمثيل الصحيح للكيانات السبعة وتسمية الحقول.<br>- استخدام `[ForeignKey]` و `[Index(..., IsUnique = true)]`.<br>- ضبط علاقة 1:1 و 1:N و N:M الصريحة. | **25 درجة** |
| **2) إعدادات الـ DbContext وقواعد الحذف** | - كتابة الـ DbSets لجميع الجداول.<br>- ضبط Fluent API لتطبيق `Cascade` على المراجعات والوسيط، وتطبيق `Restrict` على الأقسام. | **15 درجة** |
| **3) هندسة كائنات نقل البيانات (DTOs)** | - الفصل الكامل بين الـ Entities والـ DTOs.<br>- تخصيص DTOs واضحة للإنشاء والتعديل والإرجاع الشامل. | **15 درجة** |
| **4) برمجة الـ Controllers و Eager Loading** | - تنفيذ عمليات الـ CRUD الكاملة للمنتجات والأقسام والوسوم.<br>- استخدام `.Include()` و `.ThenInclude()` بدون Circular Reference وبدون أخطاء Null. | **25 درجة** |
| **5) العمليات الخاصة والتعديل الجزئي** | - تنفيذ `HttpPatch` لتعديل السعر.<br>- تنفيذ إضافة المراجعات وإسناد الوسوم ديناميكياً مع منع التكرار. | **10 درجات** |
| **6) جودة ونظافة الكود وبناء المشروع** | - خلو الكود من الأخطاء والتحذيرات (`0 Errors, 0 Warnings`).<br>- سلامة وتطبيق الـ Migration في SQL Server. | **10 درجات** |
| **المجموع النهائي** | | **100 درجة** |

---

## 9. تعليمات التسليم للطلاب (Submission Guidelines)

1. يتم تسليم مستودع الكود (Git Repository) أو ملف مضغوط يحمل اسم الطالب ورقمه الجامعي.
2. يجب أن يحتوي المشروع على ملف الـ Solution كاملاً قابلاً للبناء المباشر (`dotnet build`).
3. يجب أن يحتوي المشروع على مجلد الـ Migrations الجاهز للتطبيق عبر أمر:
   ```bash
   dotnet ef database update
   ```
4. يفضل إرفاق ملف تجارب HTTP مثل `EcommerceAPI_AI.http` لاختبار جميع الـ Endpoints بسلاسة.
