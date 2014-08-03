/**********************************\
 * 房顶装修材料统计系统
 *   数据库代码
 *   2011-3-30
\**********************************/

/******************************************\
 * 配置和产品库
\******************************************/
-- 房顶类型表，用户快速生成房顶平面图
drop table if exists ceiling_samples;
create table ceiling_samples (
    ID int auto_increment primary key, 
    name varchar(64), 
    `lines` int default 4, -- 边数
    unique key (name)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

insert into ceiling_samples (name, `lines`) 
values ('方形', 4), ('L形', 6), ('双L形1', 8), ('双L形2', 8);

-- 房屋墙表 
drop table if exists ceiling_sample_walles;
create table ceiling_sample_walles (
    ID int auto_increment primary key, 
    sample_id int, 
    wallnum int,  -- 边的序号，每一个房顶模型的各条边，以这个序号顺时针组织起来
		  -- 最后一条边的终点，做为第一条边的起点
    endx float, 
    endy float, 
    radian float default 0, -- 墙是曲面时，墙的中点到两边端点组成直线的距离
    wallType enum('main', 'help') default 'help', 
    index (sample_id, wallnum)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

insert into ceiling_sample_walles (sample_id, wallnum, endx, endy)
values	(1, 1, 2, 1), (1, 2, 2, 2), (1, 3, 1, 2), (1, 4, 1, 1), 
	(2, 1, 4, 1), (2, 2, 4, 2), (2, 3, 5, 2), (2, 4, 5, 5), (2, 5, 1, 5), (2, 6, 1, 1), 
	(3, 1, 4, 1), (3, 2, 4, 2), (3, 3, 5, 2), (3, 4, 5, 4), (3, 5, 4, 4), (3, 6, 4, 5), (3, 7, 1, 5), (3, 8, 1, 1), 
	(4, 1, 4, 1), (4, 2, 4, 2), (4, 3, 5, 2), (4, 4, 5, 5), (4, 5, 2, 5), (4, 6, 2, 4), (4, 7, 1, 4), (4, 8, 1, 1);


-- 产品分类表，用户辅助对产品进行查找
drop table if exists product_classes;
create table product_classes (
    ID int auto_increment primary key,
    name varchar(255), 
    category varchar(255), 
    parent_class_id int default 0, 
    clone_id int default 0, 
    type enum('surface', 'auxiliary') default 'surface', 
                -- 用于区分该产品是否可以直接平铺在房顶上 
    index (parent_class_id)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- 产品表
drop table if exists products;
create table products (
    ID int auto_increment primary key,
    name varchar(255),  -- 产品名称
    pattern varchar(64), -- 型号
    color varchar(32),    -- 颜色
    width float default 0,  
    height float default 0, 
    `length` float default 0, 
    unit varchar(16),     -- 单位
    price float default 0, -- 单价
    remarks varchar(255),  -- 备注
    class_id int default 0, 
    photo varchar(255), 
    index (name), 
--    index (type), 
    index (class_id)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

/******************************************\
 * 业务模块
\******************************************/
-- 房顶图表
drop table if exists ceilings;
create table ceilings (
    ID int auto_increment primary key, 
    order_id int, 
    name varchar(64), 
    `lines` int default 4, -- 边数
    display_left int default 0,
    display_top int default 0,
    display_width int default 0,
    display_height int default 0,
    `top` float default 0,
    `left` float default 0,
    width float default 0,
    height float default 0,
    scale float default 0,
    paint_width float default 0,
    paint_height float default 0,
    `rows` int default 0, 
    columns int default 0,
    products varchar(2048),
    appendix varchar(1024),
    keels varchar(2048),
    index (order_id), 
    index (name)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- 
drop table if exists SZones;
create table SZones (
    ID int auto_increment primary key, 
    ceiling_id int, 
    szone_num int,
    beginx float, 
    beginy float, 
    depth int,
    remark varchar(255)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- 房屋墙表 只能表示直线了标准圆弧线 其他墙请用圆弧近似
drop table if exists ceiling_walles;
create table ceiling_walles (
    ID int auto_increment primary key, 
    ceiling_id int, 
    wallnum int, -- 墙面编号，该房屋的第几条边（墙）
    endx float, 
    endy float, 
    radian float default 0, -- 墙是曲面时，墙的中点到两边端点组成直线的距离
    index (ceiling_id, wallnum)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- 订单表
drop table if exists orders;
create table orders (
    ID int auto_increment primary key, 
    number varchar(16),
    customer varchar(64), 
    address varchar(255), -- 收货地址
    phone varchar(64),    -- 联系电话
    salesman varchar(32), 
    sum_price float, 
    install_date datetime, 
    create_date datetime,
    remark varchar(255),
    index (number),
    index (customer)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- 订单出货单
drop table if exists goods;
create table goods (
    ID int auto_increment primary key, 
    order_id int, 
    product_id int, 
    model varchar(32), 
    category varchar(32),
    amount int, 
    price float default 0, -- 单价
    total float, 
    remark varchar(255), 
    index (order_id)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

create or replace view good_view (id, order_id, product_id, name, category, pattern, color, unit, price, amount, model, total, remark) 
as  select g.ID, g.order_id, g.product_id, p.name, g.category, p.pattern, p.color, p.unit, g.price, g.amount, g.model, g.total, g.remark
    from goods as g left join products as p on p.ID = g.product_id;
/*
insert into product_classes (name, parent_class_id)
values	("A示例类型", 0), ("B示例类型", 0), ("C示例类型", 0), 
	("B1子类型",  2), ("B2子类型",  2), ("B3子类型",  2), 
	("龙骨", 0), ("吊件", 0), ("吊杆", 0);

insert into products (name, pattern, color, width, height, `length`, unit, price, remarks, type, class_id, photo)
values	("示例产品一", "甲型", "白色", 50, 50, 0, "块", 10.80, "", 'surface', 1, 'icons/rectangle_blue.png'), 
	("示例产品二", "甲型", "白色", 60, 50, 0, "块", 10.80, "", 'surface', 1, 'icons/rectangle_green.png'), 
	("示例产品三", "甲型", "白色", 50, 50, 0, "块", 10.80, "", 'surface', 1, 'icons/rectangle_blue.png'), 
	("示例产品四", "丙型", "白色", 40, 50, 0, "块", 10.80, "", 'surface', 1, 'icons/rectangle_green.png'), 
	("示例产品五", "丙型", "白色", 60, 60, 0, "块", 10.80, "", 'surface', 1, 'icons/rectangle_green.png'), 
	("示例产品六", "甲型", "白色", 60, 60, 0, "块", 10.80, "", 'surface', 1, 'icons/rectangle_green.png'), 
	("示例产品七", "乙型", "白色", 60, 60, 0, "块", 10.80, "", 'surface', 3, 'icons/rectangle_green.png'), 
	("示例产品八", "丙型", "白色", 60, 50, 0, "块", 10.80, "", 'surface', 3, 'icons/rectangle_green.png'), 
	("示例产品九", "乙型", "白色", 60, 50, 0, "块", 10.80, "", 'surface', 3, 'icons/rectangle_green.png'), 
	("示例产品十", "乙型", "白色", 50, 50, 0, "块", 10.80, "", 'surface', 3, 'icons/rectangle_green.png'), 
	("示例产品十一", "乙型", "白色", 30, 50, 0, "块", 10.80, "", 'surface', 4, 'icons/rectangle_green.png'), 
	("示例产品十二", "甲型", "白色", 30, 50, 0, "块", 10.80, "", 'surface', 4, 'icons/rectangle_green.png'), 
	("示例产品十三", "乙型", "白色", 30, 50, 0, "块", 10.80, "", 'surface', 4, 'icons/rectangle_green.png'), 
	("示例产品十四", "甲型", "白色", 30, 50, 0, "块", 10.80, "", 'surface', 5, 'icons/rectangle_green.png'), 
	("示例产品十五", "乙型", "白色", 30, 50, 0, "块", 10.80, "", 'surface', 5, 'icons/rectangle_green.png'), 
	("示例产品十六", "甲型", "白色", 30, 50, 0, "块", 10.80, "", 'surface', 5, 'icons/rectangle_green.png'), 
	("示例产品十七", "乙型", "白色", 30, 50, 0, "块", 10.80, "", 'surface', 5, 'icons/rectangle_green.png'), 
	("示例产品十八", "甲型", "白色", 30, 50, 0, "块", 10.80, "", 'surface', 5, 'icons/rectangle_green.png');

insert into products (name, pattern, category, color, width, height, `length`, unit, price, remarks, type, class_id, photo)
values	("示例产品一", "甲型", "子类", "白色", 50, 50, 0, "块", 10.80, "", 'surface', 1, 'icons/rectangle_blue.png'), 
	("示例产品二", "甲型", "子类", "白色", 60, 50, 0, "块", 10.80, "", 'surface', 1, 'icons/rectangle_green.png'), 
	("示例产品三", "甲型", "子类", "白色", 50, 50, 0, "块", 10.80, "", 'surface', 1, 'icons/rectangle_blue.png'), 
	("示例产品四", "丙型", "子类", "白色", 40, 50, 0, "块", 10.80, "", 'surface', 1, 'icons/rectangle_green.png'), 
	("示例产品五", "丙型", "子类", "白色", 60, 60, 0, "块", 10.80, "", 'surface', 1, 'icons/rectangle_green.png'), 
	("示例产品六", "甲型", "子类", "白色", 60, 60, 0, "块", 10.80, "", 'surface', 1, 'icons/rectangle_green.png'), 
	("示例产品七", "乙型", "子类", "白色", 60, 60, 0, "块", 10.80, "", 'surface', 3, 'icons/rectangle_green.png'), 
	("示例产品八", "丙型", "丑类", "白色", 60, 50, 0, "块", 10.80, "", 'surface', 3, 'icons/rectangle_green.png'), 
	("示例产品九", "乙型", "丑类", "白色", 60, 50, 0, "块", 10.80, "", 'surface', 3, 'icons/rectangle_green.png'), 
	("示例产品十", "乙型", "丑类", "白色", 50, 50, 0, "块", 10.80, "", 'surface', 3, 'icons/rectangle_green.png'), 
	("示例产品十一", "乙型", "丑类", "白色", 30, 50, 0, "块", 10.80, "", 'surface', 4, 'icons/rectangle_green.png'), 
	("示例产品十二", "甲型", "子类", "白色", 30, 50, 0, "块", 10.80, "", 'surface', 4, 'icons/rectangle_green.png'), 
	("示例产品十三", "乙型", "丑类", "白色", 30, 50, 0, "块", 10.80, "", 'surface', 4, 'icons/rectangle_green.png'), 
	("示例产品十四", "甲型", "子类", "白色", 30, 50, 0, "块", 10.80, "", 'surface', 5, 'icons/rectangle_green.png'), 
	("示例产品十五", "乙型", "丑类", "白色", 30, 50, 0, "块", 10.80, "", 'surface', 5, 'icons/rectangle_green.png'), 
	("示例产品十六", "甲型", "子类", "白色", 30, 50, 0, "块", 10.80, "", 'surface', 5, 'icons/rectangle_green.png'), 
	("示例产品十七", "乙型", "丑类", "白色", 30, 50, 0, "块", 10.80, "", 'surface', 5, 'icons/rectangle_green.png'), 
	("示例产品十八", "甲型", "子类", "白色", 30, 50, 0, "块", 10.80, "", 'surface', 5, 'icons/rectangle_green.png');
*/
/*
create or replace good_view (id, order_id, product_id, name, pattern, category, unit, model, amount, price)
as select g.id, order_id, product_id, name, pattern, category, unit, model, amount, price
    from goods as g left join products as p on p.id = g.product_id;
*/
/*
create or replace view report (id, order_id, product_id, name, category, pattern, color, unit, price, amount, model, total, remark, customer, address, phone, salesman, sum_price, install_date, create_date) 
as  select g.ID, g.order_id, g.product_id, p.name, c.name as category, p.pattern, p.color, p.unit, p.price, g.amount, g.model, g.total, g.remark, o.customer, o.address, o.phone, o.salesman, o.sum_price, o.install_date, o.create_date 
    from goods as g left join products as p on p.ID = g.product_id left join product_classes as c on p.class_id = c.id left join orders as o on g.order_id = o.id;
*/
