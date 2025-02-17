var express = require('express');
var router = express.Router();

//增加引用函式
const class1 = require('./utility/class');
 
//接收POST請求
router.post('/', function(req, res, next) {
    var school = req.body.school;                
    var name = req.body.name;        
    var email = req.session.passport.user.emails[0].value;      
    var max_number = Number(req.body.max_number);          
    var topics = req.body.topics;

    if ( typeof topics === 'string' ) {
        topics = [topics];
    }
    // 建立一個新資料物件
    var newData={
        // class_id:class_id,
        school:school,
        name:name,
        email:email,
        max_number:max_number,
        topics:topics
    }
    class1.add(newData).then(d => {
        if (d==0){
            res.redirect('back');
            //res.render('class');  //傳至成功頁面
        }else{
            //res.render('class');     //導向錯誤頁面
        }  
    })
});

module.exports = router;